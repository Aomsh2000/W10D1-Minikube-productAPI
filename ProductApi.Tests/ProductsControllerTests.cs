using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductApi.Controllers;
using ProductApi.Models;
using ProductApi.Services;
using Xunit;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ProductApi.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _controller = new ProductsController(_mockProductService.Object, _mockConfiguration.Object);
        }

        [Fact]
        public void GetAll_ReturnsOk_WithListOfProducts()
        {
            // Arrange
            var products = new List<Product> { new Product { Id = 1, Name = "Test" } };
            _mockProductService.Setup(s => s.GetAll()).Returns(products);

            // Act
            var result = _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result); // ✅ Use result.Result
            Assert.Equal(products, okResult.Value);
        }


        [Fact]
        public void GetById_ExistingId_ReturnsProduct()
        {
            var product = new Product { Id = 1, Name = "Sample" };
            _mockProductService.Setup(s => s.GetById(1)).Returns(product);

            var result = _controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(product, okResult.Value);
        }

        [Fact]
        public void GetById_NonExistingId_ReturnsNotFound()
        {
            _mockProductService.Setup(s => s.GetById(999)).Returns((Product)null);

            var result = _controller.GetById(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void Add_ReturnsCreatedAtAction()
        {
            var product = new Product { Id = 2, Name = "New Product" };

            var result = _controller.Add(product);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetById", createdResult.ActionName);
            Assert.Equal(product, createdResult.Value);
        }

        [Fact]
        public void GetConfig_ReturnsAppNameAndCurrency()
        {
            _mockConfiguration.Setup(c => c["AppName"]).Returns("MyApp");
            _mockConfiguration.Setup(c => c["DefaultCurrency"]).Returns("USD");

            var result = _controller.GetConfig();

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic config = okResult.Value!;
            Assert.Equal("MyApp", config.AppName);
            Assert.Equal("USD", config.DefaultCurrency);
        }
    }
}
