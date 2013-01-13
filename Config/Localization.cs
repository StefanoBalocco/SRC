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

using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace SRC.Config
{
    public class Localization
    {
        public delegate void NewLanguage();
        public event NewLanguage ChangeLanguage;

        private Assembly assembly;

        public Localization(Assembly assembly)
        {
            this.assembly = assembly;
        }

        public Localization()
            : this( Assembly.GetEntryAssembly( ) )
        {
        }

        private ResourceManager _resourceManager;
        public ResourceManager resourceManager
        {
            get
            {
                if( null == _resourceManager )
                {
                    _resourceManager = new ResourceManager( assembly.GetName( ).Name + ".language", assembly );
                }
                return _resourceManager;
            }
        }

        private CultureInfo _defaultCultureInfo;
        public CultureInfo defaultCultureInfo
        {
            private get
            {
                if( null == _defaultCultureInfo )
                {
                    _defaultCultureInfo = new CultureInfo( "en" );
                }
                return _defaultCultureInfo;
            }
            set
            {
                _defaultCultureInfo = value;
            }
        }

        private CultureInfo _cultureInfo;
        public CultureInfo cultureInfo
        {
            private get
            {
                if( null == _cultureInfo )
                {
                    cultureInfo = new CultureInfo( ConfigManager.GetStringSetting( "language", Application.CurrentCulture.Name ) );
                    if( null == _cultureInfo )
                    {
                        cultureInfo = defaultCultureInfo;
                    }
                }
                return _cultureInfo;
            }
            set
            {
                if( null != resourceManager.GetString( "Language", value ) )
                {
                    _cultureInfo = value;
                    Thread.CurrentThread.CurrentCulture = _cultureInfo;
                    Thread.CurrentThread.CurrentUICulture = _cultureInfo;
                    ChangeLanguage( );
                }
            }
        }

        private string GetString(string name, string defaultValue)
        {
            string returnValue = resourceManager.GetString( name, cultureInfo );
            if( null != returnValue )
            {
                return returnValue;
            }
            return defaultValue;
        }

        public string GetString(string name)
        {
            return GetString( name, resourceManager.GetString( name, defaultCultureInfo ) );
        }
    }
}
