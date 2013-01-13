//
//  Author:
//    Stefano Balocco Stefano.Balocco@gmail.com
//
//  Copyright (c) 2011-2013, Stefano Balocco
//
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in
//       the documentation and/or other materials provided with the distribution.
//     * Neither the name of the author nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

using System.Windows.Forms;
using Microsoft.Win32;

namespace SRC.Config
{
    public class RegistryManager
    {
        public static string GetStringSetting(string key, string defaultValue)
        {
            RegistryKey registryKeyCompany = Registry.CurrentUser.OpenSubKey( "SOFTWARE", false ).OpenSubKey( Application.CompanyName, false );
            if( registryKeyCompany != null )
            {
                RegistryKey registryKeyApplication = registryKeyCompany.OpenSubKey( Application.ProductName, true );
                if( registryKeyApplication != null )
                {
                    object returnValue = registryKeyApplication.GetValue( key );
                    if( null != returnValue )
                    {
                        return (string) returnValue;
                    }
                }
            }
            return defaultValue;
        }

        public static void SetStringSetting(string key, string stringValue)
        {
            RegistryKey registryKeyCompany = Registry.CurrentUser.OpenSubKey( "SOFTWARE", true ).CreateSubKey( Application.CompanyName );
            if( null != registryKeyCompany )
            {
                RegistryKey registryKeyApplication = registryKeyCompany.CreateSubKey( Application.ProductName );
                if( null != registryKeyApplication )
                {
                    registryKeyApplication.SetValue( key, stringValue );
                }
            }
        }
    }
}
