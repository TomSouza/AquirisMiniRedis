using AquirisMiniRedis;
using FluentAssertions;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    public class RedisControllerTests : IntegrationTest
    {
        [Fact]
        public async Task Command_SetKey_ShouldReturnOk()
        {
            // Act
            var result = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.Set, "teste", "valor"));

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Command_SetKeyWithInvalidArguments_ShouldReturnBadrequest()
        {
            // Act
            var result = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.Set, "teste", ""));

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Command_SetKeyWithExpire_ShouldReturnOk()
        {
            // Act
            var result = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.SetEx, "teste", "valor", "5"));

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Command_GetKey_ShouldReturnOk()
        {
            // Arrange
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.Set, "teste", "valor"));

            // Act
            var result = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.Get, "teste"));

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Command_GetInexistentKey_ShouldReturnNotFound()
        {
            // Act
            var result = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.Get, "teste"));

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Content.ReadAsStringAsync().Result.Should().Be("(nil)");
        }

        [Fact]
        public async Task Command_DbSize_ShouldReturnOk()
        {
            // Arrange
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.Set, "teste", "valor"));

            // Act
            var result = await TestClient.GetAsync(ApiRoutes.RedisCommands.DbSize);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Should().Be("1");
        }

        [Fact]
        public async Task Command_DbSize_ShouldReturnOkWithEmptySize()
        {
            // Act
            var result = await TestClient.GetAsync(ApiRoutes.RedisCommands.DbSize);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Should().Be("0");
        }

        [Fact]
        public async Task Command_DelKey_ShouldReturnOk()
        {
            // Arrange
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.Set, "teste", "valor"));

            // Act
            var result = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.Del, "teste"));

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Command_DelMultipleKeys_ShouldReturnOk()
        {
            // Arrange
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.Set, "teste", "valor"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.Set, "teste2", "valor2"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.Set, "teste3", "valor3"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.Set, "teste4", "valor4"));

            // Act
            var result = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.Del, "teste") + " teste2 teste3 teste4");

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Command_DelKeyWithInvalidArguments_ShouldReturnBadRequest()
        {
            // Act
            var result = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.Del, ""));

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Command_IncrsKey_ShouldReturnOkAndKeyPlusOne()
        {
            // Arrange
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.Set, "teste", "10"));

            // Act
            var result = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.Incr, "teste"));

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Should().Be("(integer) 11");
        }

        [Fact]
        public async Task Command_IncrsKeyWithInvalidValue_ShouldReturnBadRequestAndErrorMessage()
        {
            // Arrange
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.Set, "teste", "valor"));

            // Act
            var result = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.Incr, "teste"));

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsStringAsync().Result.Should().Be("(error)value is not an integer or out of range");
        }

        [Fact]
        public async Task Command_ZAddKey_ShouldReturnOk()
        {
            // Act
            var result = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "1", "valor"));

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Command_ZAddKeyWithInvalidArguments_ShouldReturnBadRequest()
        {
            // Act
            var result = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "1", ""));

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Command_ZCardKey_ShouldReturnOk()
        {
            // Arrange
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "1", "valor"));

            // Act
            var result = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZCard, "testeZ"));

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Should().Be("(integer) 1");
        }

        [Fact]
        public async Task Command_ZRangeKey_ShouldReturnOk()
        {
            // Arrange
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "1", "valor"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "1", "Valor"));

            // Act
            var result = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZRange, "testeZ", "0", "-1"));

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Should().Be("1) Valor" + Environment.NewLine + "2) valor");
        }

        [Fact]
        public async Task Command_ZRangeKeyFiveFrist_ShouldReturnOk()
        {
            // Arrange
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "1", "valor"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "2", "valor2"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "3", "valor3"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "4", "valor4"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "5", "valor5"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "6", "valor6"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "7", "valor7"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "8", "valor8"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "9", "valor9"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "10", "valor10"));

            // Act
            var result = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZRange, "testeZ", "0", "5"));

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Should().Be(
                "1) valor" + Environment.NewLine
                + "2) valor2" + Environment.NewLine
                + "3) valor3" + Environment.NewLine
                + "4) valor4" + Environment.NewLine
                + "5) valor5"
            );
        }

        [Fact]
        public async Task Command_ZRankKey_ShouldReturnOk()
        {
            // Arrange
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "1", "valor"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "2", "valor2"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "3", "valor3"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "4", "valor4"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "5", "valor5"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "6", "valor6"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "7", "valor7"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "8", "valor8"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "9", "valor9"));
            _ = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZAdd, "testeZ", "10", "valor10"));

            // Act
            var result = await TestClient.GetAsync(String.Format(ApiRoutes.RedisCommands.ZRank, "testeZ", "valor5"));

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Should().Be("(integer) 4");
        }
    }
}
