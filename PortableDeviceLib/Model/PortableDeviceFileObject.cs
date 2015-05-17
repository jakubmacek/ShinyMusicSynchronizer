using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableDeviceLib.Model
{
    /// <summary>
    /// Represents physical file
    /// </summary>
    public class PortableDeviceFileObject : PortableDeviceObject
    {
        public PortableDeviceFileObject(string id) : base(id)
        {
        }

        /// <summary>
        /// Actual filename
        /// </summary>
        public string FileName { get; internal set; }

        public int Size { get; internal set; }
    }
}
