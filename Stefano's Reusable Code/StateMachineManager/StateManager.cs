//
//  Author:
//    Stefano Balocco Stefano.Balocco@gmail.com
//
//  Copyright (c) 2010-2013, Stefano Balocco
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

#if DEBUG
using System;
using System.Reflection;
#endif

namespace SRC.StateMachineManager
{
    public class StateManager
    {
        public delegate void StateChangeEventHandler( object sender, StateChangeEventArgs e );
        public event StateChangeEventHandler StateChanging;
        public event StateChangeEventHandler StateChanged;

        private int state;
        public const int StateInitial = 0;
        public const int stateFinal   = -1;

        public int State
        {
            get
            {
                return state;
            }
            set
            {
#if DEBUG
                Console.Error.WriteLine( "Debug: I'm " + Assembly.GetCallingAssembly( ).GetType( ).FullName + " and this is the state number: " + state );
#endif
                if( null != StateChanging )
                {
                    StateChanging( this, new StateChangeEventArgs( state, value ) );
                }
                int oldState = state;
                state = value;
#if DEBUG
                Console.Error.WriteLine( "Debug: I'm " + Assembly.GetCallingAssembly( ).GetType( ).FullName + " and this is the state number: " + state );
#endif
                if( null != StateChanged )
                {
                    StateChanged( this, new StateChangeEventArgs( oldState, state ) );
                }
            }
        }

        public StateManager( )
        {
            state = StateInitial;
        }

#if DEBUG
        public void printState()
        {
            Console.Error.WriteLine( "Debug: I'm " + Assembly.GetCallingAssembly( ).GetType( ).FullName + " and this is the state number: " + state );
        }
#endif

    }
}