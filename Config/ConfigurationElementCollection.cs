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

using System.Configuration;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using SRC;

namespace SRC.Config
{
    [ConfigurationCollection( typeof( ConfigurationElement ) )]
    public class ConfigurationElementCollection<T> : ConfigurationElementCollection, ToList<string> where T : ConfigurationElement, new( )
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new T( );
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ( (T) ( element ) ).ToString( );
        }

        public new int Count
        {
            get
            {
                return base.Count;
            }
        }

        public void Add(T item)
        {
            BaseAdd( item );
        }

        public void Remove(T item)
        {
            BaseRemove( item );
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt( index );
        }

        public int IndexOf(T item)
        {
            return BaseIndexOf( item );
        }

        public void Clear()
        {
            BaseClear( );
        }

        public T this[ int index ]
        {
            get
            {
                if( index >= base.Count )
                {
                    return null;
                }
                return (T) BaseGet( index );
            }
            set
            {
                if( index < base.Count )
                {
                    BaseAdd( index, value );
                }
            }
        }

        new public T this[ string name ]
        {
            get
            {
                return (T) BaseGet( name );
            }
        }

        protected override string ElementName
        {
            get
            {
                return "item";
            }
        }

        public void Insert(int index, T item)
        {
            BaseAdd( index, item );
        }

        public bool Contains(T item)
        {
            if( BaseIndexOf( item ) > -1 )
            {
                return true;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            base.CopyTo( array, arrayIndex );
        }

        public new bool IsReadOnly
        {
            get
            {
                return base.IsReadOnly( );
            }
        }

        public IList<string> ToList()
        {
            IList<string> returnValue = new List<string>( );
            foreach( T item in this )
            {
                returnValue.Add( item.ToString( ) );
            }
            return returnValue;
        }
    }
}
