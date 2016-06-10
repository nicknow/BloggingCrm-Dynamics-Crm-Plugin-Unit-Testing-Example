using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using ThinkCrm.Core.Interfaces;

namespace BloggingCrm.Account.Plugin
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PreImageValidatorAttribute : Attribute, IPluginValidator
    {
        private readonly string _imageName;
        private readonly string _logicalName;
        private readonly bool _throwException;
        private readonly List<string> _requiredAttributes;

        public PreImageValidatorAttribute(string imageName, string logicalName = null, bool throwException = true, params string[] requiredAttributes)
        {
            if (imageName == null) throw new ArgumentNullException(nameof(imageName));
            _imageName = imageName;
            _logicalName = logicalName;
            _throwException = throwException;

            _requiredAttributes = requiredAttributes.ToList();
        }

        public bool Validate(IPluginExecutionContext context, out bool throwException, out string errorMessage)
        {

            if (!context.PreEntityImages.Contains(_imageName))
            {
                throwException = _throwException;
                errorMessage = $"Required Pre-Image named {_imageName} is missing.";
                return false;
            }

            var imageEntity = context.PreEntityImages[_imageName];

            if (!string.IsNullOrEmpty(_logicalName) && !imageEntity.LogicalName.Equals(_logicalName))
            {
                throwException = _throwException;
                errorMessage =
                    $"Required Pre-Image named {_imageName} is has logical name mismatch. Expected: {_logicalName} / Actual: {imageEntity.LogicalName}";
                return false;
            }

            if (_requiredAttributes.Any(x => !imageEntity.Contains(x)))
            {
                var missingList = _requiredAttributes.Where(x => !imageEntity.Contains(x)).ToList();

                throwException = _throwException;
                errorMessage =
                    $"Required Pre-Image named {_imageName} is missing required attributes: {string.Join(", ", missingList)}.";
                return false;
            }

            throwException = false;
            errorMessage = String.Empty;
            return true;
        }
    }
}
