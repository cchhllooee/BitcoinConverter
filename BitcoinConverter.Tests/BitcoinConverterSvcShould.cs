using System;
using Xunit;
using BitcoinConverter.Code;

namespace BitcoinConverter.Tests
{
    public class BitcoinConverterSvcShould
    {
        [Fact]
        public void GetExchangeRate_USD_ReturnsUSDExchangeRate()
        {
            //arrange
            var converterSvc = new ConverterSvc();

            //act
            var exchangeRate = converterSvc.GetExchangeRate("USD");

            //assertion
            var expected = 100;
            Assert.Equal(expected, exchangeRate );
        }

        [Fact]
        public void GetExchangeRate_GBP_ReturnsUGBPExchangeRate()
        {
            //arrange
            var converterSvc = new ConverterSvc();

            //act
            var exchangeRate = converterSvc.GetExchangeRate("GBP");

            //assertion
            var expected = 200;
            Assert.Equal(expected, exchangeRate );
        }

        [Fact]
        public void GetExchangeRate_EUR_ReturnsEURExchangeRate()
        {
            //arrange
            var converterSvc = new ConverterSvc();

            //act
            var exchangeRate = converterSvc.GetExchangeRate("EUR");

            //assertion
            var expected = 300;
            Assert.Equal(expected, exchangeRate );
        }

        [Fact]
        public void ConvertBitcoins_Coin_ReturnsBitcoins()
        {
            //Arrange
            var convert = new ConverterSvc();

            //Act
            var act = convert.ConvertBitcoin(7, "EUR");

            //Assert
            var expected = 2100;
            Assert.Equal(expected,act);
        }
    }
}