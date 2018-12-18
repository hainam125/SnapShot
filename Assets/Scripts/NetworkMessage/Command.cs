using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkMessage
{
    public struct Command
    {
        private static Dictionary<KeyCode, byte> map = new Dictionary<KeyCode, byte>()
        {
            { KeyCode.W, 0 },
            { KeyCode.S, 1 },
            { KeyCode.D, 2 },
            { KeyCode.A, 3 },
            { KeyCode.Space, 4 },
        };
        public byte keyCode;
        public long id;
        public long tick;

        public Command(long cmdId, long currentTick, KeyCode code)
        {
            tick = currentTick;
            id = cmdId;
            keyCode = map[code];
        }
    }
}
