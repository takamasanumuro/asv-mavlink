﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Asv.Mavlink.Shell {
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Templates {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Templates() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Asv.Mavlink.Shell.Templates", typeof(Templates).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot;?&gt;
        ///&lt;mavlink&gt;
        ///  &lt;version&gt;3&lt;/version&gt;
        ///  &lt;dialect&gt;0&lt;/dialect&gt;
        ///  &lt;enums&gt;
        ///    &lt;enum name=&quot;MAV_AUTOPILOT&quot;&gt;
        ///      &lt;description&gt;Micro air vehicle / autopilot classes. This identifies the individual model.&lt;/description&gt;
        ///      &lt;entry value=&quot;0&quot; name=&quot;MAV_AUTOPILOT_GENERIC&quot;&gt;
        ///        &lt;description&gt;Generic autopilot, full support for everything&lt;/description&gt;
        ///      &lt;/entry&gt;
        ///      &lt;entry value=&quot;1&quot; name=&quot;MAV_AUTOPILOT_RESERVED&quot;&gt;
        ///        &lt;description&gt;Reserved for future use.&lt;/description&gt;
        ///       [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string common {
            get {
                return ResourceManager.GetString("common", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] csharp {
            get {
                object obj = ResourceManager.GetObject("csharp", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to mavgen-net.exe gen -t=standard.xml -i=in -o=out -e=cs csharp.tpl.
        /// </summary>
        internal static string generate {
            get {
                return ResourceManager.GetString("generate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] README {
            get {
                object obj = ResourceManager.GetObject("README", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot;?&gt;
        ///&lt;mavlink&gt;
        ///  &lt;!-- MAVLink standard messages --&gt;
        ///  &lt;include&gt;common.xml&lt;/include&gt;
        ///  &lt;dialect&gt;0&lt;/dialect&gt;
        ///  &lt;!-- use common.xml enums --&gt;
        ///  &lt;enums/&gt;
        ///  &lt;!-- use common.xml messages --&gt;
        ///  &lt;messages/&gt;
        ///&lt;/mavlink&gt;
        ///.
        /// </summary>
        internal static string standard {
            get {
                return ResourceManager.GetString("standard", resourceCulture);
            }
        }
    }
}
