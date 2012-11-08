﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace DevicesModule.Validation
{
    partial class Validator
    {
        void ValidateDirections()
        {
            foreach (var direction in _firesecConfiguration.DeviceConfiguration.Directions)
            {
                if (ValidateDirectionZonesContent(direction))
                { }
            }
        }

        bool ValidateDirectionZonesContent(Direction direction)
        {
            if (direction.ZoneUIDs.IsNotNullOrEmpty() == false)
            {
                _errors.Add(new DirectionValidationError(direction, "В направлении тушения нет ни одной зоны", ValidationErrorLevel.CannotWrite));
                return false;
            }
            return true;
        }
    }
}