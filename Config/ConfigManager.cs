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

using System;
using System.Configuration;
using System.IO;

namespace SRC.Config
{
    public static class ConfigManager
    {
        private static Configuration _instance;
        public static Configuration instance
        {
            get
            {
                if( null == _instance )
                {
                    if( null != System.Environment.GetEnvironmentVariable( "U3_APP_DATA_PATH" ) )
                    {
                        AppDomain.CurrentDomain.SetData( "APP_CONFIG_FILE", Path.Combine( System.Environment.GetEnvironmentVariable( "U3_APP_DATA_PATH" ), "USBBackup.config" ) );
                        _instance = ConfigurationManager.OpenExeConfiguration( ConfigurationUserLevel.None );
                    }
                    else
                    {
                        _instance = ConfigurationManager.OpenExeConfiguration( ConfigurationUserLevel.PerUserRoamingAndLocal );
                    }
                }
                return _instance;
            }
        }

        public static string GetStringSetting(string name, string defaultValue)
        {
            string returnValue = defaultValue;
            if( null != ConfigManager.instance.AppSettings.Settings[ name ] )
            {
                returnValue = ConfigManager.instance.AppSettings.Settings[ name ].Value;
            }
            return returnValue;
        }

        public static void SetStringSetting(string name, string value)
        {
            if( null != ConfigManager.instance.AppSettings.Settings[ name ] )
            {
                ConfigManager.instance.AppSettings.Settings[ name ].Value = value;
            }
            else
            {
                ConfigManager.instance.AppSettings.Settings.Add( new KeyValueConfigurationElement( name, value ) );
            }
        }
    }
}
