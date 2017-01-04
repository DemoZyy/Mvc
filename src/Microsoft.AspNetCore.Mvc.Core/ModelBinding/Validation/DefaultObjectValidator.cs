// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Validation
{
    /// <summary>
    /// The default implementation of <see cref="IObjectModelValidator"/>.
    /// </summary>
    public class DefaultObjectValidator : IObjectModelValidator
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DefaultObjectValidator"/>.
        /// </summary>
        /// <param name="modelMetadataProvider">The <see cref="IModelMetadataProvider"/>.</param>
        /// <param name="validatorProviders">The list of <see cref="IModelValidatorProvider"/>.</param>
        public DefaultObjectValidator(
            IModelMetadataProvider modelMetadataProvider,
            IList<IModelValidatorProvider> validatorProviders)
        {
            if (modelMetadataProvider == null)
            {
                throw new ArgumentNullException(nameof(modelMetadataProvider));
            }

            if (validatorProviders == null)
            {
                throw new ArgumentNullException(nameof(validatorProviders));
            }

            MetadataProvider = modelMetadataProvider;
            ValidatorProvider = new CompositeModelValidatorProvider(validatorProviders);
        }

        /// <summary>
        /// Gets the <see cref="IModelMetadataProvider"/>.
        /// </summary>
        protected IModelMetadataProvider MetadataProvider { get; }

        /// <summary>
        /// Gets the <see cref="IModelValidatorProvider"/>.
        /// </summary>
        /// <value>
        /// A <see cref="CompositeModelValidatorProvider"/> based on the <see cref="IModelValidatorProvider"/>s passed
        /// to <see cref="DefaultObjectValidator(IModelMetadataProvider, IList{IModelValidatorProvider})"/> .
        /// </value>
        protected IModelValidatorProvider ValidatorProvider { get; }

        /// <summary>
        /// Gets the <see cref="ValidatorCache"/>.
        /// </summary>
        protected ValidatorCache ValidatorCache { get; } = new ValidatorCache();

        /// <inheritdoc />
        public virtual void Validate(
            ActionContext actionContext,
            ValidationStateDictionary validationState,
            string prefix,
            object model)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            var visitor = CreateVisitor(actionContext, validationState);
            var metadata = model == null ? null : MetadataProvider.GetMetadataForType(model.GetType());
            visitor.Validate(metadata, prefix, model);
        }

        /// <summary>
        /// Creates a <see cref="ValidationVisitor"/> for the given <paramref name="actionContext"/> and
        /// <paramref name="validationState"/>.
        /// </summary>
        /// <param name="actionContext">The <see cref="ActionContext"/> associated with the current request.</param>
        /// <param name="validationState">The <see cref="ValidationStateDictionary"/>. May be <c>null</c>.</param>
        /// <returns>The new <see cref="ValidationVisitor"/>.</returns>
        protected virtual ValidationVisitor CreateVisitor(
            ActionContext actionContext,
            ValidationStateDictionary validationState)
        {
            return new ValidationVisitor(
                actionContext,
                ValidatorProvider,
                ValidatorCache,
                MetadataProvider,
                validationState);
        }
    }
}
