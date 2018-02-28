﻿namespace System.IO.BACnet
{
    // TODO you have been flagged for refactoring due to un-C#-iness
    public struct BacnetPropertyReference
    {
        public uint propertyIdentifier;
        public uint propertyArrayIndex;        /* optional */

        // TODO take BacnetPropertyIds, and default to ASN1.BACNET_ARRAY_ALL
        public BacnetPropertyReference(uint id, uint arrayIndex)
        {
            propertyIdentifier = id;
            propertyArrayIndex = arrayIndex;
        }

        public BacnetPropertyIds GetPropertyId()
        {
            return (BacnetPropertyIds)propertyIdentifier;
        }

        public override string ToString()
        {
            return $"{GetPropertyId()}";
        }
    }
}