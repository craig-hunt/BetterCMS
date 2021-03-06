﻿using System.Runtime.Serialization;

using BetterCms.Module.Api.Infrastructure;

namespace BetterCms.Module.Api.Operations.Pages.Widgets.Widget.ServerControlWidget
{
    [DataContract]
    public class ServerControlWidgetModel : ModelBase
    {
        /// <summary>
        /// Gets or sets the widget name.
        /// </summary>
        /// <value>
        /// The widget name.
        /// </value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the widget URL.
        /// </summary>
        /// <value>
        /// The widget URL.
        /// </value>
        [DataMember]
        public virtual string WidgetUrl { get; set; }

        /// <summary>
        /// Gets or sets the widget preview URL.
        /// </summary>
        /// <value>
        /// The widget preview URL.
        /// </value>
        [DataMember]
        public virtual string PreviewUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether widget is published.
        /// </summary>
        /// <value>
        /// <c>true</c> if widget is published; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsPublished { get; set; }

        /// <summary>
        /// Gets or sets the data widget was published on.
        /// </summary>
        /// <value>
        /// The date widget was published on.
        /// </value>
        [DataMember]
        public System.DateTime? PublishedOn { get; set; }

        /// <summary>
        /// Gets or sets the widget published user name.
        /// </summary>
        /// <value>
        /// The widget published user name.
        /// </value>
        [DataMember]
        public string PublishedByUser { get; set; }

        /// <summary>
        /// Gets or sets the widget category id.
        /// </summary>
        /// <value>
        /// The widget category id.
        /// </value>
        [DataMember]
        public System.Guid? CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the widget category name.
        /// </summary>
        /// <value>
        /// The widget category name.
        /// </value>
        [DataMember]
        public string CategoryName { get; set; }
    }
}