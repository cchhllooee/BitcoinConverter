using System;
using Xunit;
using BitcoinConverter.Code;
using Moq;
using Moq.Protected;
using System.Net.Http;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BitcoinConverter.Tests
{
    public class BitcoinConverterSvcShould
    {
        private const string MOCK_RESPONSE_JSON = @"{""time"": {""updated"": ""Oct 15, 2020 22:55:00 UTC"",""updatedISO"": ""2020-10-15T22:55:00+00:00"",""updateduk"": ""Oct 15, 2020 at 23:55 BST""},""chartName"": ""Bitcoin"",""bpi"": {""USD"": {""code"": ""USD"",""symbol"": ""&#36;"",""rate"": ""11,486.5341"",""description"": ""United States Dollar"",""rate_float"": 11486.5341},""GBP"": {""code"": ""GBP"",""symbol"": ""&pound;"",""rate"": ""8,900.8693"",""description"": ""British Pound Sterling"",""rate_float"": 8900.8693},""EUR"": {""code"": ""EUR"",""symbol"": ""&euro;"",""rate"": ""9,809.3278"",""description"": ""Euro"",""rate_float"": 9809.3278}}}";

        private ConverterSvc mockConverter;
        public BitcoinConverterSvcShould()
        {
            mockConverter = GetMockBitcoinConverterService();
        }
        private ConverterSvc GetMockBitcoinConverterService()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(MOCK_RESPONSE_JSON)
            };

            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object);
            var converter = new ConverterSvc(httpClient);

            return converter; 
        }

        [Fact]
        public async void GetExchangeRate_USD_ReturnsUSDExchangeRate()
        {
            //act
            var exchangeRate = await mockConverter.GetExchangeRate(ConverterSvc.Currency.USD);

            //assertion
            var expected = 11486.5341;
            Assert.Equal(expected, exchangeRate );
        }

        [Fact]
        public async void GetExchangeRate_GBP_ReturnsUGBPExchangeRate()
        {
            //act
            var exchangeRate = await mockConverter.GetExchangeRate(ConverterSvc.Currency.GBP);

            //assertion
            var expected = 8900.8693;
            Assert.Equal(expected, exchangeRate );
        }

        [Fact]
        public async void GetExchangeRate_EUR_ReturnsEURExchangeRate()
        {
            //act
            var exchangeRate = await mockConverter.GetExchangeRate(ConverterSvc.Currency.EUR);

            //assertion
            var expected = 9809.3278;
            Assert.Equal(expected, exchangeRate );
        }

        [Theory]
        [InlineData(ConverterSvc.Currency.USD, 1, 11486.5341)]
        [InlineData(ConverterSvc.Currency.USD, 2, 22973.0682)]
        [InlineData(ConverterSvc.Currency.EUR, 1, 9809.3278)]
        [InlineData(ConverterSvc.Currency.EUR, 2, 19618.6556)]
        [InlineData(ConverterSvc.Currency.GBP, 1, 8900.8693)]
        [InlineData(ConverterSvc.Currency.GBP, 2, 17801.7386)]
        public async void ConvertBitcoins_BitcoinsToCurrency_ReturnsBitcoin(ConverterSvc.Currency currency, int coins, double expected)
        {
            //Act
            var actual = await mockConverter.ConvertBitcoin(coins, currency);

            //Assert
            Assert.Equal(expected,actual);
        }

        [Fact]
        public async void ConvertBitcoins_BitcoinsAPIServiceUnavailable_ReturnNegativeOne()
        {
            //Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.ServiceUnavailable,
                Content = new StringContent("problems...")
            };

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var client = new HttpClient(handlerMock.Object);
            var converter = new ConverterSvc(client);

            //act 
            var amount = await converter.ConvertBitcoin(5, ConverterSvc.Currency.USD);


            //Assert
            var expected = -1;
            Assert.Equal(expected, amount);
        }

        [Fact]
        public async void ConvertBitcoins_BitcoinsLessThanZero_ThrowsArgumentException()
        {
            //Act
            Task result() => mockConverter.ConvertBitcoin(-1, ConverterSvc.Currency.USD);

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(result);
        }
    }
}