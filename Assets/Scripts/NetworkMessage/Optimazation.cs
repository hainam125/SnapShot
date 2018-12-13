using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkMessage
{
    public struct CompressRotation
    {
        public short a, b, c;
        public byte maxIndex;
    }

    public struct CompressPosition
    {
        public short a1, b1, c1;
        public byte a2, b2, c2;
    }

    public struct CompressPosition1
    {
        public byte a1, b1, c1;
        public byte a2, b2, c2;
    }


    public static class Optimazation
    {
        private const float ROT_FLOAT_PRECISION_MULT = 32767f / 0.707107f;
        private const float POS_FLOAT_PRECISION_MULT = 255f;
        private const float POS1_FLOAT_PRECISION_MULT = 127f;

        public static CompressPosition1 CompressPos1(Vector3 pos)
        {
            var x1 = Mathf.FloorToInt(pos.x);
            var x2 = pos.x - x1;
            var a1 = x1 + POS1_FLOAT_PRECISION_MULT;
            var a2 = x2 * POS_FLOAT_PRECISION_MULT;
            var y1 = Mathf.FloorToInt(pos.y);
            var y2 = pos.y - y1;
            var b1 = y1 + POS1_FLOAT_PRECISION_MULT;
            var b2 = y2 * POS_FLOAT_PRECISION_MULT;
            var z1 = Mathf.FloorToInt(pos.z);
            var z2 = pos.z - z1;
            var c1 = z1 + POS1_FLOAT_PRECISION_MULT;
            var c2 = z2 * POS_FLOAT_PRECISION_MULT;
            return new CompressPosition1()
            {
                a1 = (byte)a1,
                b1 = (byte)b1,
                c1 = (byte)c1,
                a2 = (byte)a2,
                b2 = (byte)b2,
                c2 = (byte)c2,
            };
        }

        public static Vector3 DecompressPos1(CompressPosition1 pos)
        {
            var a1 = (float)pos.a1 - POS1_FLOAT_PRECISION_MULT;
            var b1 = (float)pos.b1 - POS1_FLOAT_PRECISION_MULT;
            var c1 = (float)pos.c1 - POS1_FLOAT_PRECISION_MULT;
            var a2 = pos.a2 / POS_FLOAT_PRECISION_MULT;
            var b2 = pos.b2 / POS_FLOAT_PRECISION_MULT;
            var c2 = pos.c2 / POS_FLOAT_PRECISION_MULT;
            return new Vector3(a1 + a2, b1 + b2, c1 + c2);
        }
        public static CompressPosition CompressPos(Vector3 pos)
        {
            var a1 = Mathf.FloorToInt(pos.x);
            var a = pos.x - a1;
            var a2 = a * POS_FLOAT_PRECISION_MULT;
            var b1 = Mathf.FloorToInt(pos.y);
            var b = pos.y - b1;
            var b2 = b * POS_FLOAT_PRECISION_MULT;
            var c1 = Mathf.FloorToInt(pos.z);
            var c = pos.z - c1;
            var c2 = c * POS_FLOAT_PRECISION_MULT;
            return new CompressPosition()
            {
                a1 = (short)a1,
                b1 = (short)b1,
                c1 = (short)c1,
                a2 = (byte)a2,
                b2 = (byte)b2,
                c2 = (byte)c2,
            };
        }

        public static Vector3 DecompressPos(CompressPosition pos)
        {
            var a1 = (float)pos.a1;
            var b1 = (float)pos.b1;
            var c1 = (float)pos.c1;
            var a2 = pos.a2 / POS_FLOAT_PRECISION_MULT;
            var b2 = pos.b2 / POS_FLOAT_PRECISION_MULT;
            var c2 = pos.c2 / POS_FLOAT_PRECISION_MULT;
            return new Vector3(a1 + a2, b1 + b2, c1 + c2);
        }

        public static Quaternion DecompressRot(CompressRotation rotation)
        {
            var maxIndex = rotation.maxIndex;
            // Values between 4 and 7 indicate that only the index of the single field whose value is 1f was
            // sent, and (maxIndex - 4) is the correct index for that field.
            if (maxIndex >= 4 && maxIndex <= 7)
            {
                var x = (maxIndex == 4) ? 1f : 0f;
                var y = (maxIndex == 5) ? 1f : 0f;
                var z = (maxIndex == 6) ? 1f : 0f;
                var w = (maxIndex == 7) ? 1f : 0f;

                return new Quaternion(x, y, z, w);
            }
            // Read the other three fields and derive the value of the omitted field
            var a = (float)rotation.a / ROT_FLOAT_PRECISION_MULT;
            var b = (float)rotation.b / ROT_FLOAT_PRECISION_MULT;
            var c = (float)rotation.c / ROT_FLOAT_PRECISION_MULT;
            var d = Mathf.Sqrt(1f - (a * a + b * b + c * c));

            if (maxIndex == 0) return new Quaternion(d, a, b, c);
            else if (maxIndex == 1) return new Quaternion(a, d, b, c);
            else if (maxIndex == 2) return new Quaternion(a, b, d, c);

            return new Quaternion(a, b, c, d);
        }

        public static CompressRotation CompressRot(Quaternion rotation)
        {
            var a = (short)0;
            var b = (short)0;
            var c = (short)0;
            var maxIndex = (byte)0;
            var maxValue = float.MinValue;
            var sign = 1f;

            // Determine the index of the largest (absolute value) element in the Quaternion.
            // We will transmit only the three smallest elements, and reconstruct the largest
            // element during decoding. 
            for (int i = 0; i < 4; i++)
            {
                var element = rotation[i];
                var abs = Mathf.Abs(rotation[i]);
                if (abs > maxValue)
                {
                    // We don't need to explicitly transmit the sign bit of the omitted element because you 
                    // can make the omitted element always positive by negating the entire quaternion if 
                    // the omitted element is negative (in quaternion space (x,y,z,w) and (-x,-y,-z,-w) 
                    // represent the same rotation.), but we need to keep track of the sign for use below.
                    sign = (element < 0) ? -1 : 1;

                    // Keep track of the index of the largest element
                    maxIndex = (byte)i;
                    maxValue = abs;
                }
            }

            // If the maximum value is approximately 1f (such as Quaternion.identity [0,0,0,1]), then we can 
            // reduce storage even further due to the fact that all other fields must be 0f by definition, so 
            // we only need to send the index of the largest field.
            if (Mathf.Approximately(maxValue, 1f))
            {
                // Again, don't need to transmit the sign since in quaternion space (x,y,z,w) and (-x,-y,-z,-w) 
                // represent the same rotation. We only need to send the index of the single element whose value
                // is 1f in order to recreate an equivalent rotation on the receiver.
                return new CompressRotation() { maxIndex = (byte)(maxIndex + 4) };
            }
            else
            {

                // We multiply the value of each element by QUAT_PRECISION_MULT before converting to 16-bit integer 
                // in order to maintain precision. This is necessary since by definition each of the three smallest 
                // elements are less than 1.0, and the conversion to 16-bit integer would otherwise truncate everything 
                // to the right of the decimal place. This allows us to keep five decimal places.

                if (maxIndex == 0)
                {
                    a = (short)(rotation.y * sign * ROT_FLOAT_PRECISION_MULT);
                    b = (short)(rotation.z * sign * ROT_FLOAT_PRECISION_MULT);
                    c = (short)(rotation.w * sign * ROT_FLOAT_PRECISION_MULT);
                }
                else if (maxIndex == 1)
                {
                    a = (short)(rotation.x * sign * ROT_FLOAT_PRECISION_MULT);
                    b = (short)(rotation.z * sign * ROT_FLOAT_PRECISION_MULT);
                    c = (short)(rotation.w * sign * ROT_FLOAT_PRECISION_MULT);
                }
                else if (maxIndex == 2)
                {
                    a = (short)(rotation.x * sign * ROT_FLOAT_PRECISION_MULT);
                    b = (short)(rotation.y * sign * ROT_FLOAT_PRECISION_MULT);
                    c = (short)(rotation.w * sign * ROT_FLOAT_PRECISION_MULT);
                }
                else
                {
                    a = (short)(rotation.x * sign * ROT_FLOAT_PRECISION_MULT);
                    b = (short)(rotation.y * sign * ROT_FLOAT_PRECISION_MULT);
                    c = (short)(rotation.z * sign * ROT_FLOAT_PRECISION_MULT);
                }
                return new CompressRotation() { a = a, b = b, c = c, maxIndex = maxIndex };
            }
        }
    }
}
