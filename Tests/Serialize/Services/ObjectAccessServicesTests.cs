﻿using System.Collections.Generic;
using System.IO.BACnet.Serialize;
using System.IO.BACnet.Tests.TestData;
using NUnit.Framework;
using static System.IO.BACnet.Tests.Helper;

namespace System.IO.BACnet.Tests.Serialize
{
    [TestFixture]
    class ObjectAccessServicesTests
    {
        [Test]
        public void should_encode_addlistelementrequest_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            var data = new List<BacnetValue>
            {
                new BacnetValue(BacnetApplicationTags.BACNET_APPLICATION_TAG_OBJECT_PROPERTY_REFERENCE,
                    new BacnetObjectPropertyReference(new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 15),
                        new BacnetPropertyReference(BacnetPropertyIds.PROP_PRESENT_VALUE),
                        new BacnetPropertyReference(BacnetPropertyIds.PROP_RELIABILITY)))
            };

            // example taken from ANNEX F - Examples of APDU Encoding - F.3.1
            var expectedBytes = new byte[]
            {
                0x00, 0x02, 0x01, 0x08, 0x0C, 0x02, 0xC0, 0x00, 0x03, 0x19, 0x35, 0x3E, 0x0C, 0x00, 0x00, 0x00, 0x0F,
                0x1E, 0x09, 0x55, 0x09, 0x67, 0x1F, 0x3F
            };

            // act
            APDU.EncodeConfirmedServiceRequest(buffer,
                BacnetConfirmedServices.SERVICE_CONFIRMED_ADD_LIST_ELEMENT, BacnetMaxSegments.MAX_SEG0,
                BacnetMaxAdpu.MAX_APDU206, 1);

            ObjectAccessServices.EncodeAddOrRemoveListElement(buffer,
                new BacnetObjectId(BacnetObjectTypes.OBJECT_GROUP, 3),
                (uint) BacnetPropertyIds.PROP_LIST_OF_GROUP_MEMBERS, ASN1.BACNET_ARRAY_ALL, data);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_addlistelementrequest_ack_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.3.1
            var expectedBytes = new byte[] { 0x20, 0x01, 0x08 };

            // act
            APDU.EncodeSimpleAck(buffer, BacnetConfirmedServices.SERVICE_CONFIRMED_ADD_LIST_ELEMENT, 1);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_removelistelementrequest_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            var data = new List<BacnetValue>
            {
                new BacnetValue(BacnetApplicationTags.BACNET_APPLICATION_TAG_OBJECT_PROPERTY_REFERENCE,
                    new BacnetObjectPropertyReference(new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 12),
                        new BacnetPropertyReference(BacnetPropertyIds.PROP_PRESENT_VALUE),
                        new BacnetPropertyReference(BacnetPropertyIds.PROP_RELIABILITY),
                        new BacnetPropertyReference(BacnetPropertyIds.PROP_DESCRIPTION))),

                new BacnetValue(BacnetApplicationTags.BACNET_APPLICATION_TAG_OBJECT_PROPERTY_REFERENCE,
                    new BacnetObjectPropertyReference(new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 13),
                        new BacnetPropertyReference(BacnetPropertyIds.PROP_PRESENT_VALUE),
                        new BacnetPropertyReference(BacnetPropertyIds.PROP_RELIABILITY),
                        new BacnetPropertyReference(BacnetPropertyIds.PROP_DESCRIPTION)))
            };

            // example taken from ANNEX F - Examples of APDU Encoding - F.3.2
            var expectedBytes = new byte[]
            {
                0x00, 0x02, 0x34, 0x09, 0x0C, 0x02, 0xC0, 0x00, 0x03, 0x19, 0x35, 0x3E, 0x0C, 0x00, 0x00, 0x00, 0x0C,
                0x1E, 0x09, 0x55, 0x09, 0x67, 0x09, 0x1C, 0x1F, 0x0C, 0x00, 0x00, 0x00, 0x0D, 0x1E, 0x09, 0x55, 0x09,
                0x67, 0x09, 0x1C, 0x1F, 0x3F
            };

            // act
            APDU.EncodeConfirmedServiceRequest(buffer,
                BacnetConfirmedServices.SERVICE_CONFIRMED_REMOVE_LIST_ELEMENT, BacnetMaxSegments.MAX_SEG0,
                BacnetMaxAdpu.MAX_APDU206, 52);

            ObjectAccessServices.EncodeAddOrRemoveListElement(buffer,
                new BacnetObjectId(BacnetObjectTypes.OBJECT_GROUP, 3),
                (uint)BacnetPropertyIds.PROP_LIST_OF_GROUP_MEMBERS, ASN1.BACNET_ARRAY_ALL, data);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_removelistelementrequest_ack_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.3.2
            var expectedBytes = new byte[] { 0x20, 0x34, 0x09 };

            // act
            APDU.EncodeSimpleAck(buffer, BacnetConfirmedServices.SERVICE_CONFIRMED_REMOVE_LIST_ELEMENT, 52);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_createobjectrequest_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            var data = new List<BacnetPropertyValue>
            {
                new BacnetPropertyValue
                {
                    property = new BacnetPropertyReference(BacnetPropertyIds.PROP_OBJECT_NAME),
                    value = new List<BacnetValue> {new BacnetValue("Trend 1")}
                },
                new BacnetPropertyValue
                {
                    property = new BacnetPropertyReference(BacnetPropertyIds.PROP_FILE_ACCESS_METHOD),
                    value = new List<BacnetValue> {new BacnetValue(BacnetFileAccessMethod.RECORD_ACCESS)}
                }
            };

            // example taken from ANNEX F - Examples of APDU Encoding - F.3.3
            var expectedBytes = new byte[]
            {
                0x00, 0x04, 0x56, 0x0A, 0x0E, 0x09, 0x0A, 0x0F, 0x1E, 0x09, 0x4D, 0x2E, 0x75, 0x08, 0x00, 0x54, 0x72,
                0x65, 0x6E, 0x64, 0x20, 0x31, 0x2F, 0x09, 0x29, 0x2E, 0x91, 0x00, 0x2F, 0x1F
            };

            // act
            APDU.EncodeConfirmedServiceRequest(buffer,
                BacnetConfirmedServices.SERVICE_CONFIRMED_CREATE_OBJECT, BacnetMaxSegments.MAX_SEG0,
                BacnetMaxAdpu.MAX_APDU1024, 86);

            ObjectAccessServices.EncodeCreateObject(buffer, BacnetObjectTypes.OBJECT_FILE, data);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_createobjectrequest_ack_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.3.3
            var expectedBytes = new byte[] { 0x30, 0x56, 0x0A, 0xC4, 0x02, 0x80, 0x00, 0x0D };

            // act
            APDU.EncodeComplexAck(buffer, BacnetConfirmedServices.SERVICE_CONFIRMED_CREATE_OBJECT, 86);

            ObjectAccessServices.EncodeCreateObjectAcknowledge(buffer,
                new BacnetObjectId(BacnetObjectTypes.OBJECT_FILE, 13));

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_deleteobjectrequest_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.3.4
            var expectedBytes = new byte[] {0x00, 0x04, 0x57, 0x0B, 0xC4, 0x02, 0xC0, 0x00, 0x06};

            // act
            APDU.EncodeConfirmedServiceRequest(buffer,
                BacnetConfirmedServices.SERVICE_CONFIRMED_DELETE_OBJECT, BacnetMaxSegments.MAX_SEG0,
                BacnetMaxAdpu.MAX_APDU1024, 87);

            ObjectAccessServices.EncodeDeleteObject(buffer, new BacnetObjectId(BacnetObjectTypes.OBJECT_GROUP, 6));

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_deleteobjectrequest_ack_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.3.4
            var expectedBytes = new byte[] { 0x20, 0x57, 0x0B };

            // act
            APDU.EncodeSimpleAck(buffer, BacnetConfirmedServices.SERVICE_CONFIRMED_DELETE_OBJECT, 87);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_deleteobjectrequest_fail_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.3.4
            var expectedBytes = new byte[] {0x50, 0x58, 0x0B, 0x91, 0x01, 0x91, 0x17};

            // act
            APDU.EncodeError(buffer, BacnetPduTypes.PDU_TYPE_ERROR,
                BacnetConfirmedServices.SERVICE_CONFIRMED_DELETE_OBJECT, 88);
            ASN1.EncodeError(buffer, BacnetErrorClasses.ERROR_CLASS_OBJECT,
                BacnetErrorCodes.ERROR_CODE_OBJECT_DELETION_NOT_PERMITTED);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_readpropertyrequest_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.3.5
            var expectedBytes = new byte[] {0x00, 0x00, 0x01, 0x0C, 0x0C, 0x00, 0x00, 0x00, 0x05, 0x19, 0x55};

            // act
            APDU.EncodeConfirmedServiceRequest(buffer,
                BacnetConfirmedServices.SERVICE_CONFIRMED_READ_PROPERTY, BacnetMaxSegments.MAX_SEG0,
                BacnetMaxAdpu.MAX_APDU50, 1);

            ObjectAccessServices.EncodeReadProperty(buffer,
                new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 5), BacnetPropertyIds.PROP_PRESENT_VALUE);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_readpropertyrequest_ack_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            var data = new List<BacnetValue>
            {
                new BacnetValue(72.3f)
            };

            // example taken from ANNEX F - Examples of APDU Encoding - F.3.5
            var expectedBytes = new byte[]
                {0x30, 0x01, 0x0C, 0x0C, 0x00, 0x00, 0x00, 0x05, 0x19, 0x55, 0x3E, 0x44, 0x42, 0x90, 0x99, 0x9A, 0x3F};

            // act
            APDU.EncodeComplexAck(buffer, BacnetConfirmedServices.SERVICE_CONFIRMED_READ_PROPERTY, 1);
            ObjectAccessServices.EncodeReadPropertyAcknowledge(buffer,
                new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 5), BacnetPropertyIds.PROP_PRESENT_VALUE,
                data);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_readpropertymultiplerequest_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            var data = new List<BacnetPropertyReference>
            {
                new BacnetPropertyReference(BacnetPropertyIds.PROP_PRESENT_VALUE),
                new BacnetPropertyReference(BacnetPropertyIds.PROP_RELIABILITY)
            };

            // example taken from ANNEX F - Examples of APDU Encoding - F.3.7
            var expectedBytes = new byte[]
                {0x00, 0x04, 0xF1, 0x0E, 0x0C, 0x00, 0x00, 0x00, 0x10, 0x1E, 0x09, 0x55, 0x09, 0x67, 0x1F};

            // act
            APDU.EncodeConfirmedServiceRequest(buffer,
                BacnetConfirmedServices.SERVICE_CONFIRMED_READ_PROP_MULTIPLE, BacnetMaxSegments.MAX_SEG0,
                BacnetMaxAdpu.MAX_APDU1024, 241);

            ObjectAccessServices.EncodeReadPropertyMultiple(buffer,
                new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 16), data);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_readpropertymultiplerequest_ack_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            var data = new List<BacnetReadAccessResult>
            {
                new BacnetReadAccessResult(new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 16),
                    new List<BacnetPropertyValue>
                    {
                        new BacnetPropertyValue
                        {
                            property = new BacnetPropertyReference(BacnetPropertyIds.PROP_PRESENT_VALUE),
                            value = new List<BacnetValue> {new BacnetValue(72.3f)}
                        },
                        new BacnetPropertyValue
                        {
                            property = new BacnetPropertyReference(BacnetPropertyIds.PROP_RELIABILITY),
                            value = new List<BacnetValue>
                            {
                                new BacnetValue(BacnetReliability.RELIABILITY_NO_FAULT_DETECTED)
                            }
                        }
                    })
            };

            // example taken from ANNEX F - Examples of APDU Encoding - F.3.7
            var expectedBytes = new byte[]
            {
                0x30, 0xF1, 0x0E, 0x0C, 0x00, 0x00, 0x00, 0x10, 0x1E, 0x29, 0x55, 0x4E, 0x44, 0x42, 0x90, 0x99, 0x9A,
                0x4F, 0x29, 0x67, 0x4E, 0x91, 0x00, 0x4F, 0x1F
            };

            // act
            APDU.EncodeComplexAck(buffer, BacnetConfirmedServices.SERVICE_CONFIRMED_READ_PROP_MULTIPLE, 241);
            ObjectAccessServices.EncodeReadPropertyMultipleAcknowledge(buffer, data);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_readpropertymultiplerequest_formultipleobjects_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            var data = new List<BacnetReadAccessSpecification>
            {
                new BacnetReadAccessSpecification(new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 33),
                    new List<BacnetPropertyReference>
                    {
                        new BacnetPropertyReference(BacnetPropertyIds.PROP_PRESENT_VALUE)
                    }),

                new BacnetReadAccessSpecification(new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 50),
                    new List<BacnetPropertyReference>
                    {
                        new BacnetPropertyReference(BacnetPropertyIds.PROP_PRESENT_VALUE)
                    }),

                new BacnetReadAccessSpecification(new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 35),
                    new List<BacnetPropertyReference>
                    {
                        new BacnetPropertyReference(BacnetPropertyIds.PROP_PRESENT_VALUE)
                    }),
            };

            // example taken from ANNEX F - Examples of APDU Encoding - F.3.7
            var expectedBytes = new byte[]
            {
                0x00, 0x04, 0x02, 0x0E, 0x0C, 0x00, 0x00, 0x00, 0x21, 0x1E, 0x09, 0x55, 0x1F, 0x0C, 0x00, 0x00, 0x00,
                0x32, 0x1E, 0x09, 0x55, 0x1F, 0x0C, 0x00, 0x00, 0x00, 0x23, 0x1E, 0x09, 0x55, 0x1F
            };

            // act
            APDU.EncodeConfirmedServiceRequest(buffer,
                BacnetConfirmedServices.SERVICE_CONFIRMED_READ_PROP_MULTIPLE, BacnetMaxSegments.MAX_SEG0,
                BacnetMaxAdpu.MAX_APDU1024, 2);

            ObjectAccessServices.EncodeReadPropertyMultiple(buffer, data);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_readpropertymultiplerequest_formultipleobjects_ack_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            var data = new List<BacnetReadAccessResult>
            {
                new BacnetReadAccessResult(new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 33),
                    new List<BacnetPropertyValue>
                    {
                        new BacnetPropertyValue
                        {
                            property = new BacnetPropertyReference(BacnetPropertyIds.PROP_PRESENT_VALUE),
                            value = new List<BacnetValue> {new BacnetValue(42.3f)}
                        }
                    }),

                new BacnetReadAccessResult(new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 50),
                    new List<BacnetPropertyValue>
                    {
                        new BacnetPropertyValue
                        {
                            property = new BacnetPropertyReference(BacnetPropertyIds.PROP_PRESENT_VALUE),
                            value = new List<BacnetValue>
                            {
                                new BacnetValue(new BacnetError(BacnetErrorClasses.ERROR_CLASS_OBJECT,
                                    BacnetErrorCodes.ERROR_CODE_UNKNOWN_OBJECT))
                            }
                        }
                    }),

                new BacnetReadAccessResult(new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 35),
                    new List<BacnetPropertyValue>
                    {
                        new BacnetPropertyValue
                        {
                            property = new BacnetPropertyReference(BacnetPropertyIds.PROP_PRESENT_VALUE),
                            value = new List<BacnetValue> {new BacnetValue(435.7f)}
                        }
                    })
            };

            // example taken from ANNEX F - Examples of APDU Encoding - F.3.7
            var expectedBytes = new byte[]
            {
                0x30, 0x02, 0x0E, 0x0C, 0x00, 0x00, 0x00, 0x21, 0x1E, 0x29, 0x55, 0x4E, 0x44, 0x42, 0x29, 0x33, 0x33,
                0x4F, 0x1F, 0x0C, 0x00, 0x00, 0x00, 0x32, 0x1E, 0x29, 0x55, 0x5E, 0x91, 0x01, 0x91, 0x1F, 0x5F, 0x1F,
                0x0C, 0x00, 0x00, 0x00, 0x23, 0x1E, 0x29, 0x55, 0x4E, 0x44, 0x43, 0xD9, 0xD9, 0x9A, 0x4F, 0x1F
            };

            // act
            APDU.EncodeComplexAck(buffer, BacnetConfirmedServices.SERVICE_CONFIRMED_READ_PROP_MULTIPLE, 2);
            ObjectAccessServices.EncodeReadPropertyMultipleAcknowledge(buffer, data);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_readrangerequest_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.3.8
            var expectedBytes = new byte[]
            {
                0x02, 0x02, 0x01, 0x1A, 0x0C, 0x05, 0x00, 0x00, 0x01, 0x19, 0x83, 0x7E, 0xA4, 0x62, 0x03, 0x17, 0x01,
                0xB4, 0x13, 0x34, 0x22, 0x00, 0x31, 0x04, 0x7F
            };

            // act
            APDU.EncodeConfirmedServiceRequest(buffer,
                BacnetConfirmedServices.SERVICE_CONFIRMED_READ_RANGE, BacnetMaxSegments.MAX_SEG0,
                BacnetMaxAdpu.MAX_APDU206, 1,
                type: BacnetPduTypes.PDU_TYPE_CONFIRMED_SERVICE_REQUEST | BacnetPduTypes.SEGMENTED_RESPONSE_ACCEPTED);

            ObjectAccessServices.EncodeReadRange(buffer, new BacnetObjectId(BacnetObjectTypes.OBJECT_TRENDLOG, 1),
                BacnetPropertyIds.PROP_LOG_BUFFER, BacnetReadRangeRequestTypes.RR_BY_TIME, 0,
                new DateTime(1998, 3, 23, 19, 52, 34), 4);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_readrangerequest_ack_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();
            var data = ASHRAE.F_3_8();

            // example taken from ANNEX F - Examples of APDU Encoding - F.3.8
            var expectedBytes = new byte[]
            {
                0x30, 0x01, 0x1A, 0x0C, 0x05, 0x00, 0x00, 0x01, 0x19, 0x83, 0x3A, 0x05, 0xC0, 0x49, 0x02, 0x5E, 0x0E,
                0xA4, 0x62, 0x03, 0x17, 0x01, 0xB4, 0x13, 0x36, 0x1B, 0x00, 0x0F, 0x1E, 0x2C, 0x41, 0x90, 0x00, 0x00,
                0x1F, 0x2A, 0x04, 0x00, 0x0E, 0xA4, 0x62, 0x03, 0x17, 0x01, 0xB4, 0x13, 0x38, 0x1B, 0x00, 0x0F, 0x1E,
                0x2C, 0x41, 0x90, 0xCC, 0xCD, 0x1F, 0x2A, 0x04, 0x00, 0x5F, 0x6B, 0x01, 0x35, 0x61
            };

            // act
            APDU.EncodeComplexAck(buffer, BacnetConfirmedServices.SERVICE_CONFIRMED_READ_RANGE, 1);

            var applicationData = new EncodeBuffer();
            ObjectAccessServices.EncodeLogRecord(applicationData, data.Record1);
            ObjectAccessServices.EncodeLogRecord(applicationData, data.Record2);

            ObjectAccessServices.EncodeReadRangeAcknowledge(buffer, data.ObjectId, data.PropertyId, data.Flags,
                data.ItemCount, applicationData.ToArray(), data.RequestType, data.FirstSequence);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_decode_multiple_logrecords()
        {
            // arrange
            var buffer = new EncodeBuffer();
            var data = ASHRAE.F_3_8();

            // act
            ObjectAccessServices.EncodeLogRecord(buffer, data.Record1);
            ObjectAccessServices.EncodeLogRecord(buffer, data.Record2);
            ObjectAccessServices.DecodeLogRecord(buffer.buffer, 0, buffer.GetLength(), (int)data.ItemCount, out var decodedRecords);

            /*
             * Debug - write packet to network to analyze in WireShark
             *

            var client = new BacnetClient(new BacnetIpUdpProtocolTransport(47808, true));
            client.Start();
            client.ReadRangeResponse(new BacnetAddress(BacnetAddressTypes.IP, "192.168.1.99"), 42, null,
                new BacnetObjectId(BacnetObjectTypes.OBJECT_TRENDLOG, 1),
                new BacnetPropertyReference((uint) BacnetPropertyIds.PROP_LOG_BUFFER, ASN1.BACNET_ARRAY_ALL),
                BacnetResultFlags.LAST_ITEM, 2, buffer.ToArray(), BacnetReadRangeRequestTypes.RR_BY_TIME, 79201);
            */

            // assert
            Assert.That(decodedRecords.Length, Is.EqualTo(2));
            Assert.That(data.Record1, Is.Not.SameAs(decodedRecords[0]));
            Assert.That(data.Record2, Is.Not.SameAs(decodedRecords[1]));
            AssertPropertiesAndFieldsAreEqual(data.Record1, decodedRecords[0]);
            AssertPropertiesAndFieldsAreEqual(data.Record2, decodedRecords[1]);
        }

        [Test]
        public void should_encode_writepropertyrequest_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.3.9
            var expectedBytes = new byte[]
            {
                0x00, 0x04, 0x59, 0x0F, 0x0C, 0x00, 0x80, 0x00, 0x01, 0x19, 0x55, 0x3E, 0x44, 0x43, 0x34, 0x00, 0x00,
                0x3F
            };

            // act
            APDU.EncodeConfirmedServiceRequest(buffer, BacnetConfirmedServices.SERVICE_CONFIRMED_WRITE_PROPERTY,
                BacnetMaxSegments.MAX_SEG0, BacnetMaxAdpu.MAX_APDU1024, 89);

            ObjectAccessServices.EncodeWriteProperty(buffer,
                new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_VALUE, 1), BacnetPropertyIds.PROP_PRESENT_VALUE,
                new List<BacnetValue> {new BacnetValue(180f)});

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_writepropertyrequest_ack_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.3.9
            var expectedBytes = new byte[] { 0x20, 0x59, 0x0F };

            // act
            APDU.EncodeSimpleAck(buffer, BacnetConfirmedServices.SERVICE_CONFIRMED_WRITE_PROPERTY, 89);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_writepropertymultiplerequest_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();
            var data = new[]
            {
                new BacnetWriteAccessSpecification(
                    new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_VALUE, 5),
                    A(new BacnetWriteAccessSpecification.Property(BacnetPropertyIds.PROP_PRESENT_VALUE,
                        new BacnetValue(67f)))),

                new BacnetWriteAccessSpecification(
                    new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_VALUE, 6),
                    A(new BacnetWriteAccessSpecification.Property(BacnetPropertyIds.PROP_PRESENT_VALUE,
                        new BacnetValue(67f)))),

                new BacnetWriteAccessSpecification(
                    new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_VALUE, 7),
                    A(new BacnetWriteAccessSpecification.Property(BacnetPropertyIds.PROP_PRESENT_VALUE,
                        new BacnetValue(72f)))),
            };

            // example taken from ANNEX F - Examples of APDU Encoding - F.3.10
            var expectedBytes = new byte[]
            {
                0x00, 0x04, 0x01, 0x10, 0x0C, 0x00, 0x80, 0x00, 0x05, 0x1E, 0x09, 0x55, 0x2E, 0x44, 0x42, 0x86, 0x00,
                0x00, 0x2F, 0x1F, 0x0C, 0x00, 0x80, 0x00, 0x06, 0x1E, 0x09, 0x55, 0x2E, 0x44, 0x42, 0x86, 0x00, 0x00,
                0x2F, 0x1F, 0x0C, 0x00, 0x80, 0x00, 0x07, 0x1E, 0x09, 0x55, 0x2E, 0x44, 0x42, 0x90, 0x00, 0x00, 0x2F,
                0x1F
            };

            // act
            APDU.EncodeConfirmedServiceRequest(buffer, BacnetConfirmedServices.SERVICE_CONFIRMED_WRITE_PROP_MULTIPLE,
                BacnetMaxSegments.MAX_SEG0, BacnetMaxAdpu.MAX_APDU1024, 1);

            ObjectAccessServices.EncodeWritePropertyMultiple(buffer, data);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_writepropertymultiplerequest_ack_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.3.10
            var expectedBytes = new byte[] { 0x20, 0x01, 0x10 };

            // act
            APDU.EncodeSimpleAck(buffer, BacnetConfirmedServices.SERVICE_CONFIRMED_WRITE_PROP_MULTIPLE, 1);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }
    }
}
