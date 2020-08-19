using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

using React;
using Microsoft.AspNetCore.Html;
using IHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;

namespace React.Sample.Webpack.CoreMvc
{
    public static class ReactServer
    {
        // The below snippet of code is referred from 
        // https://github.com/reactjs/React.NET/blob/master/src/React.AspNet/HtmlHelperExtensions.cs
        // To enable calling the react render methods from 
        // within a controller, as it is only available as an Html helper in the
        // ReactJS.net library

        [ThreadStatic]
        private static StringWriter _sharedStringWriter;
          
        /// <summary>
        /// Gets the React environment
        /// </summary>
        private static IReactEnvironment Environment
        {
            get
            {
                return ReactEnvironment.GetCurrentOrThrow;
            }
        }

        

        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
        };

        /// <summary>
		/// Renders the specified React component.
        /// This method is provided for advanced use cases,
        /// where custom render functions and other options
        /// are needed.
		/// </summary>
		/// <typeparam name="T">Type of the props</typeparam>
		/// <param name="componentName">Name of the component</param>
		/// <param name="props">Props to initialise the component with</param>
		/// <param name="htmlTag">HTML tag to wrap the component in. Defaults to &lt;div&gt;</param>
		/// <param name="containerId">ID to use for the container HTML tag. Defaults to an auto-generated ID</param>
		/// <param name="clientOnly">Skip rendering server-side and only output client-side initialisation code. Defaults to <c>false</c></param>
		/// <param name="serverOnly">Skip rendering React specific data-attributes, container and client-side initialisation during server side rendering. Defaults to <c>false</c></param>
		/// <param name="containerClass">HTML class(es) to set on the container tag</param>
		/// <param name="exceptionHandler">A custom exception handler that will be called if a component throws during a render. Args: (Exception ex, string componentName, string containerId)</param>
		/// <param name="renderFunctions">Functions to call during component render</param>
		/// <returns>The component's HTML</returns>
		public static string Render<T>(
            string componentName,
            T props,
            string htmlTag = null,
            string containerId = "root",
            bool clientOnly = false,
            bool serverOnly = false,
            string containerClass = null,
            Action<Exception, string, string> exceptionHandler = null,
            IRenderFunctions renderFunctions = null
        )
        {
            try
            {
                var reactComponent = Environment.CreateComponent(componentName, props, containerId, clientOnly, serverOnly);
                if (!string.IsNullOrEmpty(htmlTag))
                {
                    reactComponent.ContainerTag = htmlTag;
                }

                if (!string.IsNullOrEmpty(containerClass))
                {
                    reactComponent.ContainerClass = containerClass;
                }

                return RenderToString(writer => reactComponent.RenderHtml(writer, clientOnly, serverOnly, exceptionHandler ?? ReactRenderExceptionHandler, renderFunctions));
            }
            finally
            {
                Environment.ReturnEngineToPool();
            }
        }

        private static string RenderToString(Action<StringWriter> withWriter)
        {
            var stringWriter = _sharedStringWriter;
            if (stringWriter != null)
            {
                stringWriter.GetStringBuilder().Clear();
            }
            else
            {
                _sharedStringWriter = stringWriter = new StringWriter(new StringBuilder(128));
            }

            withWriter(stringWriter);
            return stringWriter.ToString();
        }
        private static void ReactRenderExceptionHandler(Exception ex, string componentName, string containerId)
        {
            Console.WriteLine(componentName + " " + containerId);
           Console.WriteLine(ex);
        }
        private static string DefaultComponentName(string componentName, bool isMobile)
        {
            if (string.IsNullOrWhiteSpace(componentName))
            {
                return isMobile ? "Components.RootComponent" : "Components.DesktopRootComponent";
            }
            return componentName;
        }
    }
}
