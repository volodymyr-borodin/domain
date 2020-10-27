using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public sealed class DomainResult
    {
        private DomainResult()
            : this(Enumerable.Empty<DomainError>())
        { }

        private DomainResult(IEnumerable<DomainError> errors)
        {
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        public IEnumerable<DomainError> Errors { get; }

        public bool Successed => !Errors.Any();

        /// <summary>
        /// Build success result
        /// </summary>
        public static DomainResult Success() => new DomainResult(Enumerable.Empty<DomainError>());

        /// <summary>
        /// Build unsuccessful result
        /// </summary>
        public static DomainResult Error(string error) => Error(new[] { error }.Select(e => new DomainError(e)));

        /// <summary>
        /// Build unsuccessful result
        /// </summary>
        public static DomainResult Error(DomainError error) => Error(new[] { error });

        /// <summary>
        /// Build unsuccessful result
        /// </summary>
        public static DomainResult Error(IEnumerable<DomainError> errors) => new DomainResult(errors);
    }
}