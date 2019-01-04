using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkMessage
{
    public struct Command
    {
        public static Dictionary<KeyCode, byte> Keys = new Dictionary<KeyCode, byte>()
        {
            { KeyCode.W, 1 },
            { KeyCode.S, 2 },
            { KeyCode.D, 4 },
            { KeyCode.A, 8 },
            { KeyCode.Space, 16 },
        };

        public byte keyCode;
        public long id;
        public long tick;

        public Command(long cmdId, long currentTick, byte code)
        {
            tick = currentTick;
            id = cmdId;
            keyCode = code;
        }

        public bool HasUp() { return Has(KeyCode.W); }

        public bool HasDown() { return Has(KeyCode.S); }

        public bool HasLeft() { return Has(KeyCode.A); }

        public bool HasRight() { return Has(KeyCode.D); }

        private bool Has(KeyCode key) { return (keyCode & Keys[key]) == Keys[key]; }
    }
}
