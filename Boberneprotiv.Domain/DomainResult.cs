using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Domain;

public sealed record DomainResult
{
    private static DomainResult SuccessObject => new DomainResult("Success", true, ImmutableDictionary<string, object>.Empty);
    public static DomainResult Success => SuccessObject;
    public static DomainResult Error(string error) => new DomainResult(error, false, ImmutableDictionary<string, object>.Empty);
    public static DomainResult Error(string error, IReadOnlyDictionary<string, object> details) => new DomainResult(error, false, details);

    private DomainResult(string message, bool isSuccess, IReadOnlyDictionary<string, object> details)
    {
        Message = message;
        IsSuccess = isSuccess;
        Details = details;
    }

    public string Message { get; }
    public bool IsSuccess { get; }
    public IReadOnlyDictionary<string, object> Details { get; }

    
    public static Func<IEnumerable<DomainResult>, string> ConstMessageStrategy(string message = "There are several problems") => _ => message;
    public static Func<IEnumerable<DomainResult>, string> ConcatMessageStrategy(string separator = ". ") => results => string.Join(". ", results.Select(r => r.Message));

    public static IImmutableDictionary<string, object> FirstDetailsStrategy(IEnumerable<DomainResult> results)
    {
        var result = new Dictionary<string, object>();
        foreach (var (key, value) in results.SelectMany(r => r.Details))
        {
            if (!result.ContainsKey(key))
            {
                result[key] = value;
            }
        }

        return result.ToImmutableDictionary();
    }

    public static IImmutableDictionary<string, object> LastDetailsStrategy(IEnumerable<DomainResult> results)
    {
        var result = new Dictionary<string, object>();
        foreach (var (key, value) in results.SelectMany(r => r.Details))
        {
            result[key] = value;
        }

        return result.ToImmutableDictionary();
    }

    public static IImmutableDictionary<string, object> ListDetailsStrategy(IEnumerable<DomainResult> results)
    {
        var result = new Dictionary<string, object>();
        foreach (var (key, value) in results.SelectMany(r => r.Details))
        {
            if (!result.ContainsKey(key))
            {
                result[key] = new List<object>();
            }

            ((List<object>) result[key]).Add(value);
        }

        return result.ToImmutableDictionary();
    }

    public static DomainResult MergeErrors(IEnumerable<DomainResult> results,
        Func<IEnumerable<DomainResult>, string> messageStrategy,
        Func<IEnumerable<DomainResult>, IImmutableDictionary<string, object>> detailsStrategy)
    {
        results = results ?? throw new ArgumentNullException(nameof(results));
        messageStrategy = messageStrategy ?? throw new ArgumentNullException(nameof(messageStrategy));
        detailsStrategy = detailsStrategy ?? throw new ArgumentNullException(nameof(detailsStrategy));

        var errors = results.Where(r => !r.IsSuccess).ToList();
        return errors.Count switch
        {
            0 => Success,
            1 => errors[0],
            _ => new DomainResult(messageStrategy(errors),
                false,
                detailsStrategy(errors))
        };
    }
}
