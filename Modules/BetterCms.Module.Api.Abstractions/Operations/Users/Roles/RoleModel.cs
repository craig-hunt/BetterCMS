﻿using System.Runtime.Serialization;

using BetterCms.Module.Api.Infrastructure;

namespace BetterCms.Module.Api.Operations.Users.Roles
{
    [DataContract]
    public class RoleModel : ModelBase
    {
        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        /// <value>
        /// The name of the role.
        /// </value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether role is systematic.
        /// </summary>
        /// <value>
        /// <c>true</c> if role is systematic; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsSystematic { get; set; }
    }
}