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

        public bool hasUp()
        {
            return has(KeyCode.W);
        }
        public bool hasDown()
        {
            return has(KeyCode.S);
        }
        public bool hasLeft()
        {
            return has(KeyCode.A);
        }
        public bool hasRight()
        {
            return has(KeyCode.D);
        }
        private bool has(KeyCode key)
        {
            return (keyCode & Keys[key]) == Keys[key];
        }
    }
}
