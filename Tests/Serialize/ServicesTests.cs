﻿using System.Collections.Generic;
using System.IO.BACnet.EventNotification;
using System.IO.BACnet.EventNotification.EventValues;
using System.IO.BACnet.Serialize;
using NUnit.Framework;

namespace System.IO.BACnet.Tests.Serialize
{
    [TestFixture]
    public class ServicesTests
    {
        [Test]
        public void should_encode_logrecord_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();
            var record1 = new BacnetLogRecord(BacnetTrendLogValueType.TL_TYPE_REAL, 18.0,
                new DateTime(1998, 3, 23, 19, 54, 27), 0);
            var record2 = new BacnetLogRecord(BacnetTrendLogValueType.TL_TYPE_REAL, 18.1,
                new DateTime(1998, 3, 23, 19, 56, 27), 0);

            // example taken from ANNEX F - Examples of APDU Encoding
            var expectedBytes = new byte[]
            {
                0x0E, 0xA4, 0x62, 0x03, 0x17, 0x01, 0xB4, 0x13, 0x36, 0x1B, 0x00, 0x0F,
                0x1E, 0x2C, 0x41, 0x90, 0x00, 0x00, 0x1F, 0x2A, 0x04, 0x00, 0x0E, 0xA4,
                0x62, 0x03, 0x17, 0x01, 0xB4, 0x13, 0x38, 0x1B, 0x00, 0x0F, 0x1E, 0x2C,
                0x41, 0x90, 0xCC, 0xCD, 0x1F, 0x2A, 0x04, 0x00
            };

            // act
            Services.EncodeLogRecord(buffer, record1);
            Services.EncodeLogRecord(buffer, record2);
            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_decode_multiple_logrecords()
        {
            // arrange
            var buffer = new EncodeBuffer();
            var record1 = new BacnetLogRecord(BacnetTrendLogValueType.TL_TYPE_REAL, 18.0,
                new DateTime(1998, 3, 23, 19, 54, 27), 0);
            var record2 = new BacnetLogRecord(BacnetTrendLogValueType.TL_TYPE_REAL, 18.1,
                new DateTime(1998, 3, 23, 19, 56, 27), 0);

            // act
            Services.EncodeLogRecord(buffer, record1);
            Services.EncodeLogRecord(buffer, record2);
            Services.DecodeLogRecord(buffer.buffer, 0, buffer.GetLength(), 2, out var decodedRecords);

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
            Helper.AssertPropertiesAndFieldsAreEqual(record1, decodedRecords[0]);
            Helper.AssertPropertiesAndFieldsAreEqual(record2, decodedRecords[1]);
        }

        [Test]
        public void should_encode_confirmendcovnotificationrequest_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();
            var data = new[]
            {
                new BacnetPropertyValue
                {
                    property = new BacnetPropertyReference((uint) BacnetPropertyIds.PROP_PRESENT_VALUE,
                        ASN1.BACNET_ARRAY_ALL),
                    value = new List<BacnetValue> {new BacnetValue(65.0f)}
                },
                new BacnetPropertyValue
                {
                    property = new BacnetPropertyReference((uint) BacnetPropertyIds.PROP_STATUS_FLAGS,
                        ASN1.BACNET_ARRAY_ALL),
                    value = new List<BacnetValue> {new BacnetValue(BacnetBitString.Parse("0000"))}
                }
            };

            // example taken from ANNEX F - Examples of APDU Encoding - F.1.2
            var expectedBytes = new byte[]
            {
                0x00, 0x02, 0x0F, 0x01,

                0x09, 0x12, 0x1C, 0x02, 0x00, 0x00, 0x04, 0x2C, 0x00, 0x00, 0x00, 0x0A,
                0x39, 0x00, 0x4E, 0x09, 0x55, 0x2E, 0x44, 0x42, 0x82, 0x00, 0x00, 0x2F,
                0x09, 0x6F, 0x2E, 0x82, 0x04, 0x00, 0x2F, 0x4F
            };

            // act
            APDU.EncodeConfirmedServiceRequest(buffer, BacnetPduTypes.PDU_TYPE_CONFIRMED_SERVICE_REQUEST,
                BacnetConfirmedServices.SERVICE_CONFIRMED_COV_NOTIFICATION, BacnetMaxSegments.MAX_SEG0,
                BacnetMaxAdpu.MAX_APDU206, 15);

            Services.EncodeCOVNotifyConfirmed(buffer, 18, 4,
                new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 10), 0, data);
            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_confirmendcovnotificationrequest_ack_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.1.2
            var expectedBytes = new byte[]
            {
                0x20, 0x0F, 0x01
            };

            // act
            APDU.EncodeSimpleAck(buffer, BacnetPduTypes.PDU_TYPE_SIMPLE_ACK,
                BacnetConfirmedServices.SERVICE_CONFIRMED_COV_NOTIFICATION, 15);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_unconfirmendcovnotificationrequest_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();
            var data = new[]
            {
                new BacnetPropertyValue
                {
                    property = new BacnetPropertyReference((uint) BacnetPropertyIds.PROP_PRESENT_VALUE,
                        ASN1.BACNET_ARRAY_ALL),
                    value = new List<BacnetValue> {new BacnetValue(65.0f)}
                },
                new BacnetPropertyValue
                {
                    property = new BacnetPropertyReference((uint) BacnetPropertyIds.PROP_STATUS_FLAGS,
                        ASN1.BACNET_ARRAY_ALL),
                    value = new List<BacnetValue> {new BacnetValue(BacnetBitString.Parse("0000"))}
                }
            };

            // example taken from ANNEX F - Examples of APDU Encoding - F.1.3
            var expectedBytes = new byte[]
            {
                0x10, 0x02,

                0x09, 0x12, 0x1C, 0x02, 0x00, 0x00, 0x04, 0x2C, 0x00, 0x00, 0x00, 0x0A,
                0x39, 0x00, 0x4E, 0x09, 0x55, 0x2E, 0x44, 0x42, 0x82, 0x00, 0x00, 0x2F,
                0x09, 0x6F, 0x2E, 0x82, 0x04, 0x00, 0x2F, 0x4F
            };

            // act
            APDU.EncodeUnconfirmedServiceRequest(buffer, BacnetPduTypes.PDU_TYPE_UNCONFIRMED_SERVICE_REQUEST,
                BacnetUnconfirmedServices.SERVICE_UNCONFIRMED_COV_NOTIFICATION);

            Services.EncodeCOVNotifyConfirmed(buffer, 18, 4,
                new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 10), 0, data);
            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_confirmendeventnotificationrequest_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();
            var data = new StateTransition<OutOfRange>(new OutOfRange()
            {
                ExceedingValue = 80.1f,
                StatusFlags = BacnetBitString.Parse("1000"),
                Deadband = 1.0f,
                ExceededLimit = 80.0f
            })
            {
                ProcessIdentifier = 1,
                InitiatingObjectIdentifier = new BacnetObjectId(BacnetObjectTypes.OBJECT_DEVICE, 4),
                EventObjectIdentifier = new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 2),
                TimeStamp = new BacnetGenericTime(default(DateTime), BacnetTimestampTags.TIME_STAMP_SEQUENCE, 16),
                NotificationClass = 4,
                Priority = 100,
                NotifyType = BacnetNotifyTypes.NOTIFY_ALARM,
                AckRequired = true,
                FromState = BacnetEventStates.EVENT_STATE_NORMAL,
                ToState = BacnetEventStates.EVENT_STATE_HIGH_LIMIT
            };

            // example taken from ANNEX F - Examples of APDU Encoding - F.1.4
            var expectedBytes = new byte[]
            {
                0x00, 0x02, 0x10, 0x02, 0x09, 0x01, 0x1C, 0x02, 0x00, 0x00, 0x04, 0x2C, 0x00, 0x00, 0x00, 0x02,
                0x3E, 0x19, 0x10, 0x3F, 0x49, 0x04, 0x59, 0x64, 0x69, 0x05, 0x89, 0x00, 0x99, 0x01, 0xA9, 0x00,
                0xB9, 0x03, 0xCE, 0x5E, 0x0C, 0x42, 0xA0, 0x33, 0x33, 0x1A, 0x04, 0x80, 0x2C, 0x3F, 0x80, 0x00,
                0x00, 0x3C, 0x42, 0xA0, 0x00, 0x00, 0x5F, 0xCF
            };

            // act
            APDU.EncodeConfirmedServiceRequest(buffer, BacnetPduTypes.PDU_TYPE_CONFIRMED_SERVICE_REQUEST,
                BacnetConfirmedServices.SERVICE_CONFIRMED_EVENT_NOTIFICATION, BacnetMaxSegments.MAX_SEG0,
                BacnetMaxAdpu.MAX_APDU206, 16);

            Services.EncodeEventNotifyData(buffer, data);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_confirmendeventnotificationrequest_ack_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.1.4
            var expectedBytes = new byte[]
            {
                0x20, 0x10, 0x02
            };

            // act
            APDU.EncodeSimpleAck(buffer, BacnetPduTypes.PDU_TYPE_SIMPLE_ACK,
                BacnetConfirmedServices.SERVICE_CONFIRMED_EVENT_NOTIFICATION, 16);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));

        }

        [Test]
        public void should_encode_acknowledgealarmrequest_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.1.4
            var expectedBytes = new byte[]
            {
                0x00, 0x02, 0x07, 0x00, 0x09, 0x01, 0x1C, 0x00, 0x00, 0x00, 0x02, 0x29, 0x03,
                0x3E, 0x19, 0x10, 0x3F, 0x4C, 0x00, 0x4D, 0x44, 0x4C, 0x5E, 0x2E, 0xA4, 0x5C,
                0x06, 0x15, 0x07 /* instead of FF */, 0xB4, 0x0D, 0x03, 0x29, 0x09, 0x2F, 0x5F
            };

            // act
            APDU.EncodeConfirmedServiceRequest(buffer, BacnetPduTypes.PDU_TYPE_CONFIRMED_SERVICE_REQUEST,
                BacnetConfirmedServices.SERVICE_CONFIRMED_ACKNOWLEDGE_ALARM, BacnetMaxSegments.MAX_SEG0,
                BacnetMaxAdpu.MAX_APDU206, 7);

            Services.EncodeAlarmAcknowledge(buffer, 1, new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 2),
                (uint) BacnetEventStates.EVENT_STATE_HIGH_LIMIT, "MDL",
                new BacnetGenericTime(default(DateTime), BacnetTimestampTags.TIME_STAMP_SEQUENCE, 16),
                new BacnetGenericTime(new DateTime(1992, 6, 21, 13, 3, 41).AddMilliseconds(90),
                    BacnetTimestampTags.TIME_STAMP_DATETIME));

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_acknowledgealarmrequest_ack_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.1.4
            var expectedBytes = new byte[]
            {
                0x20, 0x07, 0x00
            };

            // act
            APDU.EncodeSimpleAck(buffer, BacnetPduTypes.PDU_TYPE_SIMPLE_ACK,
                BacnetConfirmedServices.SERVICE_CONFIRMED_ACKNOWLEDGE_ALARM, 7);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));

        }

        [Test]
        public void should_encode_unconfirmedeventnotificationrequest_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();
            var data = new StateTransition<OutOfRange>(new OutOfRange()
            {
                ExceedingValue = 80.1f,
                StatusFlags = BacnetBitString.Parse("1000"),
                Deadband = 1.0f,
                ExceededLimit = 80.0f
            })
            {
                ProcessIdentifier = 1,
                InitiatingObjectIdentifier = new BacnetObjectId(BacnetObjectTypes.OBJECT_DEVICE, 9),
                EventObjectIdentifier = new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 2),
                TimeStamp = new BacnetGenericTime(default(DateTime), BacnetTimestampTags.TIME_STAMP_SEQUENCE, 16),
                NotificationClass = 4,
                Priority = 100,
                NotifyType = BacnetNotifyTypes.NOTIFY_ALARM,
                AckRequired = true,
                FromState = BacnetEventStates.EVENT_STATE_NORMAL,
                ToState = BacnetEventStates.EVENT_STATE_HIGH_LIMIT
            };

            // example taken from ANNEX F - Examples of APDU Encoding - F.1.5
            var expectedBytes = new byte[]
            {
                0x10, 0x03, 0x09, 0x01, 0x1C, 0x02, 0x00, 0x00, 0x09, 0x2C, 0x00, 0x00, 0x00, 0x02, 0x3E, 0x19, 0x10,
                0x3F, 0x49, 0x04, 0x59, 0x64, 0x69, 0x05, 0x89, 0x00, 0x99, 0x01, 0xA9, 0x00, 0xB9, 0x03, 0xCE, 0x5E,
                0x0C, 0x42, 0xA0, 0x33, 0x33, 0x1A, 0x04, 0x80, 0x2C, 0x3F, 0x80, 0x00, 0x00, 0x3C, 0x42, 0xA0, 0x00,
                0x00, 0x5F, 0xCF
            };

            // act
            APDU.EncodeUnconfirmedServiceRequest(buffer, BacnetPduTypes.PDU_TYPE_UNCONFIRMED_SERVICE_REQUEST,
                BacnetUnconfirmedServices.SERVICE_UNCONFIRMED_EVENT_NOTIFICATION);

            Services.EncodeEventNotifyData(buffer, data);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_getalarmsummary_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.1.6
            var expectedBytes = new byte[]
            {
                0x00, 0x02, 0x01, 0x03
            };

            // act
            APDU.EncodeConfirmedServiceRequest(buffer, BacnetPduTypes.PDU_TYPE_CONFIRMED_SERVICE_REQUEST,
                BacnetConfirmedServices.SERVICE_CONFIRMED_GET_ALARM_SUMMARY, BacnetMaxSegments.MAX_SEG0,
                BacnetMaxAdpu.MAX_APDU206, 1);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_getalarmsummary_ack_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.1.6

            var expectedBytes = new byte[]
            {
                0x30, 0x01, 0x03, 0xC4, 0x00, 0x00, 0x00, 0x02, 0x91, 0x03, 0x82, 0x05, 0x60, 0xC4, 0x00, 0x00, 0x00,
                0x03, 0x91, 0x04, 0x82, 0x05, 0xE0
            };

            // act
            APDU.EncodeComplexAck(buffer, BacnetPduTypes.PDU_TYPE_COMPLEX_ACK,
                BacnetConfirmedServices.SERVICE_CONFIRMED_GET_ALARM_SUMMARY, 1);

            Services.EncodeAlarmSummary(buffer, new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 2),
                BacnetEventStates.EVENT_STATE_HIGH_LIMIT, BacnetBitString.Parse("011"));

            Services.EncodeAlarmSummary(buffer, new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 3),
                BacnetEventStates.EVENT_STATE_LOW_LIMIT, BacnetBitString.Parse("111"));

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_geteventinformation_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.1.8
            var expectedBytes = new byte[]
            {
                0x00 /* docs say 0x02, but that's wrong! */, 0x02, 0x01, 0x1D
            };

            // act
            APDU.EncodeConfirmedServiceRequest(buffer, BacnetPduTypes.PDU_TYPE_CONFIRMED_SERVICE_REQUEST,
                BacnetConfirmedServices.SERVICE_CONFIRMED_GET_EVENT_INFORMATION, BacnetMaxSegments.MAX_SEG0,
                BacnetMaxAdpu.MAX_APDU206, 1);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_geteventinformation_ack_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            var data = new[]
            {
                new BacnetGetEventInformationData()
                {
                    objectIdentifier = new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 2),
                    eventState = BacnetEventStates.EVENT_STATE_HIGH_LIMIT,
                    acknowledgedTransitions = BacnetBitString.Parse("011"),
                    eventTimeStamps = new[]
                    {
                        new BacnetGenericTime(new DateTime(1, 1, 1, 15, 35, 00).AddMilliseconds(200), BacnetTimestampTags.TIME_STAMP_TIME),
                        new BacnetGenericTime(default(DateTime), BacnetTimestampTags.TIME_STAMP_TIME),
                        new BacnetGenericTime(default(DateTime), BacnetTimestampTags.TIME_STAMP_TIME),
                    },
                    notifyType = BacnetNotifyTypes.NOTIFY_ALARM,
                    eventEnable = BacnetBitString.Parse("111"),
                    eventPriorities = new uint[] {15, 15, 20}
                },
                new BacnetGetEventInformationData()
                {
                objectIdentifier = new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 3),
                eventState = BacnetEventStates.EVENT_STATE_NORMAL,
                acknowledgedTransitions = BacnetBitString.Parse("110"),
                eventTimeStamps = new[]
                {
                    new BacnetGenericTime(new DateTime(1, 1, 1, 15, 40, 00), BacnetTimestampTags.TIME_STAMP_TIME),
                    new BacnetGenericTime(default(DateTime), BacnetTimestampTags.TIME_STAMP_TIME),
                    new BacnetGenericTime(new DateTime(1, 1, 1, 15, 45, 30).AddMilliseconds(300), BacnetTimestampTags.TIME_STAMP_TIME),
                },
                notifyType = BacnetNotifyTypes.NOTIFY_ALARM,
                eventEnable = BacnetBitString.Parse("111"),
                eventPriorities = new uint[] {15, 15, 20}
                }
            };

            // example taken from ANNEX F - Examples of APDU Encoding - F.1.8
            var expectedBytes = new byte[]
            {
                0x30, 0x01, 0x1D, 0x0E, 0x0C, 0x00, 0x00, 0x00, 0x02, 0x19, 0x03, 0x2A, 0x05, 0x60, 0x3E, 0x0C, 0x0F,
                0x23, 0x00, 0x14, 0x0C, 0xFF, 0xFF, 0xFF, 0xFF, 0x0C, 0xFF, 0xFF, 0xFF, 0xFF, 0x3F, 0x49, 0x00, 0x5A,
                0x05, 0xE0, 0x6E, 0x21, 0x0F, 0x21, 0x0F, 0x21, 0x14, 0x6F, 0x0C, 0x00, 0x00, 0x00, 0x03, 0x19, 0x00,
                0x2A, 0x05, 0xC0, 0x3E, 0x0C, 0x0F, 0x28, 0x00, 0x00, 0x0C, 0xFF, 0xFF, 0xFF, 0xFF, 0x0C, 0x0F, 0x2D,
                0x1E, 0x1E, 0x3F, 0x49, 0x00, 0x5A, 0x05, 0xE0, 0x6E, 0x21, 0x0F, 0x21, 0x0F, 0x21, 0x14, 0x6F, 0x0F,
                0x19, 0x00
            };

            // act
            APDU.EncodeComplexAck(buffer, BacnetPduTypes.PDU_TYPE_COMPLEX_ACK,
                BacnetConfirmedServices.SERVICE_CONFIRMED_GET_EVENT_INFORMATION, 1);

            Services.EncodeGetEventInformationAcknowledge(buffer, data, false);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_lifesafetyoperation_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.1.9
            var expectedBytes = new byte[]
            {
                0x00, 0x02, 0x0F, 0x1B, 0x09, 0x12, 0x1C, 0x00, 0x4D, 0x44, 0x4C, 0x29, 0x04, 0x3C, 0x05, 0x40, 0x00,
                0x01
            };

            // act
            APDU.EncodeConfirmedServiceRequest(buffer, BacnetPduTypes.PDU_TYPE_CONFIRMED_SERVICE_REQUEST,
                BacnetConfirmedServices.SERVICE_CONFIRMED_LIFE_SAFETY_OPERATION, BacnetMaxSegments.MAX_SEG0,
                BacnetMaxAdpu.MAX_APDU206, 15);

            Services.EncodeLifeSafetyOperation(buffer, 18, "MDL",
                (uint) BacnetLifeSafetyOperations.LIFE_SAFETY_OP_RESET,
                new BacnetObjectId(BacnetObjectTypes.OBJECT_LIFE_SAFETY_POINT, 1));

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_lifesafetyoperation_ack_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.1.9
            var expectedBytes = new byte[]
            {
                0x20, 0x0F, 0x1B
            };

            // act
            APDU.EncodeSimpleAck(buffer, BacnetPduTypes.PDU_TYPE_SIMPLE_ACK,
                BacnetConfirmedServices.SERVICE_CONFIRMED_LIFE_SAFETY_OPERATION, 15);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_subscribecov_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.1.10
            var expectedBytes = new byte[]
                {0x00, 0x02, 0x0F, 0x05, 0x09, 0x12, 0x1C, 0x00, 0x00, 0x00, 0x0A, 0x29, 0x01, 0x39, 0x00};

            // act
            APDU.EncodeConfirmedServiceRequest(buffer, BacnetPduTypes.PDU_TYPE_CONFIRMED_SERVICE_REQUEST,
                BacnetConfirmedServices.SERVICE_CONFIRMED_SUBSCRIBE_COV, BacnetMaxSegments.MAX_SEG0,
                BacnetMaxAdpu.MAX_APDU206, 15);

            Services.EncodeSubscribeCOV(buffer, 18, new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 10),
                false, true, 0);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_subscribecov_ack_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.1.10
            var expectedBytes = new byte[]
            {
                0x20, 0x0F, 0x05
            };

            // act
            APDU.EncodeSimpleAck(buffer, BacnetPduTypes.PDU_TYPE_SIMPLE_ACK,
                BacnetConfirmedServices.SERVICE_CONFIRMED_SUBSCRIBE_COV, 15);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_subscribecovproperty_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.1.11
            var expectedBytes = new byte[]
            {
                0x00, 0x02, 0x0F, 0x1C, 0x09, 0x12, 0x1C, 0x00, 0x00, 0x00, 0x0A, 0x29, 0x01, 0x39, 0x3C, 0x4E, 0x09,
                0x55, 0x4F, 0x5C, 0x3F, 0x80, 0x00, 0x00
            };
            // act
            APDU.EncodeConfirmedServiceRequest(buffer, BacnetPduTypes.PDU_TYPE_CONFIRMED_SERVICE_REQUEST,
                BacnetConfirmedServices.SERVICE_CONFIRMED_SUBSCRIBE_COV_PROPERTY, BacnetMaxSegments.MAX_SEG0,
                BacnetMaxAdpu.MAX_APDU206, 15);

            Services.EncodeSubscribeProperty(buffer, 18, new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, 10),
                false, true, 60,
                new BacnetPropertyReference((uint) BacnetPropertyIds.PROP_PRESENT_VALUE, ASN1.BACNET_ARRAY_ALL), true,
                1.0f);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void should_encode_subscribecovproperty_ack_according_to_ashrae_example()
        {
            // arrange
            var buffer = new EncodeBuffer();

            // example taken from ANNEX F - Examples of APDU Encoding - F.1.11
            var expectedBytes = new byte[]
            {
                0x20, 0x0F, 0x1C
            };

            // act
            APDU.EncodeSimpleAck(buffer, BacnetPduTypes.PDU_TYPE_SIMPLE_ACK,
                BacnetConfirmedServices.SERVICE_CONFIRMED_SUBSCRIBE_COV_PROPERTY, 15);

            var encodedBytes = buffer.ToArray();

            // assert
            Assert.That(encodedBytes, Is.EquivalentTo(expectedBytes));
        }

        [Test]
        public void GenerateCode()
        {
            Console.WriteLine(Helper.Doc2Code(@"
X'00' PDU Type=0 (BACnet-Confirmed-Request-PDU, SEG=0, MOR=0, SA=0)
X'02' Maximum APDU Size Accepted=206 octets
X'0F' Invoke ID=15
X'1C' Service Choice=28 (SubscribeCOVProperty-Request)
X'09' SD Context Tag 0 (Subscriber Process Identifier, L=1)
X'12' 18
X'1C' SD Context Tag 1 (Monitored Object Identifier, L=4)
X'0000000A' Analog Input, Instance Number=10
X'29' SD Context Tag 2 (Issue Confirmed Notifications, L=1)
X'01' 1 (TRUE)
X'39' SD Context Tag 3 (Lifetime, L=1)
X'3C' 60
X'4E' PD Opening Tag 4 (Monitored Property, L=1)
X'09' SD Context Tag 0 (Property Identifier, L=1)
X'55' 85 (PRESENT_VALUE)
X'4F' PD Closing Tag 4 (Property Identifier)
X'5C' SD Context Tag 5 (COV Increment, L=4)
X'3F800000' 1.0
"));
        }
    }
}