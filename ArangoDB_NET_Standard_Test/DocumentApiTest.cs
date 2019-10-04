﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ArangoDB_NET_Standard;
using ArangoDB_NET_Standard.CollectionApi;
using ArangoDB_NET_Standard.DocumentApi;
using Xunit;

namespace ArangoDB_NET_Standard_Test
{
    public class DocumentApiTest: ApiTestBase
    {
        private DocumentApiClient _docClient;
        private ArangoDBClient _adb;
        private readonly string _dbName = nameof(DocumentApiTest);

        public DocumentApiTest()
        {
            CreateDatabase(_dbName);
           
            _adb = GetArangoDBClient(_dbName);
            _docClient = _adb.Document;
            try
            {
                var response = _adb.Collection.PostCollectionAsync(
                    new PostCollectionOptions
                    {
                        Name = "TestCollection"
                    })
                    .GetAwaiter()
                    .GetResult();
            }
            catch(ApiErrorException ex) when (ex.ApiError.ErrorNum == 1207)
            {
                // collection must exist already
                Console.WriteLine(ex.Message);
            }
        }

        [Fact]
        public async Task PostDocument_ShouldSucceed()
        {
            Dictionary<string, object> document = new Dictionary<string, object>{ ["key"] = "value" };
            var response = await _docClient.PostDocumentAsync("TestCollection", document);
            Assert.False(string.IsNullOrWhiteSpace(response._id));
            Assert.False(string.IsNullOrWhiteSpace(response._key));
            Assert.False(string.IsNullOrWhiteSpace(response._rev));
            Assert.Null(response.New);
            Assert.Null(response.Old);
        }

        [Fact]
        public async Task PostDocument_ShouldSucceed_WhenNewDocIsReturned()
        {
            var doc = new { test = 123 };
            var response = await _docClient.PostDocumentAsync("TestCollection", doc, new PostDocumentsOptions
            {
                ReturnNew = true
            });
            Assert.Null(response.Old);
            Assert.NotNull(response.New);
            Assert.Equal(123, (int)response.New.test);
        }

        [Fact]
        public async Task PostDocument_ShouldFail_WhenDocumentIsInvalid()
        {
            var doc = new { test = 123, _key = "Spaces are not allowed in keys" };
            ApiErrorException ex = await Assert.ThrowsAsync<ApiErrorException>(async () =>
                await _docClient.PostDocumentAsync("TestCollection", doc));

            Assert.NotNull(ex.ApiError.ErrorMessage);
        }

        [Fact]
        public async Task PostDocuments_ShouldSucceed()
        {
            var document1 = new { test = "value" };
            var document2 = new { test = "value" };
            PostDocumentsResponse<dynamic> response = await _docClient.PostDocumentsAsync("TestCollection", new dynamic[] { document1, document2 });
            Assert.Equal(2, response.Count);
            foreach (var innerResponse in response)
            {
                Assert.False(string.IsNullOrWhiteSpace(innerResponse._id));
                Assert.False(string.IsNullOrWhiteSpace(innerResponse._key));
                Assert.False(string.IsNullOrWhiteSpace(innerResponse._rev));
                Assert.Null(innerResponse.New);
                Assert.Null(innerResponse.Old);
            }
        }

        [Fact]
        public async Task PostDocuments_ShouldSucceed_WhenNewDocIsReturned()
        {
            dynamic document1 = new { test = "value" };
            dynamic document2 = new { test = "value" };
            var response = await _docClient.PostDocumentsAsync("TestCollection", new dynamic[] { document1, document2 }, new PostDocumentsOptions
            {
                ReturnNew = true
            });
            Assert.Equal(2, response.Count);
            foreach (var innerResponse in response)
            {
                Assert.False(string.IsNullOrWhiteSpace(innerResponse._id));
                Assert.False(string.IsNullOrWhiteSpace(innerResponse._key));
                Assert.False(string.IsNullOrWhiteSpace(innerResponse._rev));
                Assert.NotNull(innerResponse.New);
                Assert.Null(innerResponse.Old);
                Assert.Equal("value", (string)innerResponse.New.test);
            }
        }

        [Fact]
        public async Task PostDocuments_ShouldNotThrowButShouldReportError_WhenADocumentIsInvalid()
        {
            dynamic document1 = new { _key = "spaces are not allowed in keys" };
            dynamic document2 = new { test = "value" };
            var response = await _docClient.PostDocumentsAsync("TestCollection", new dynamic[] { document1, document2 });

            Assert.Equal(2, response.Count);
            Assert.True(response[0].Error);
            Assert.False(response[1].Error);
        }

        [Fact]
        public async Task PutDocument_ShouldSucceed()
        {
            var doc1 = new { _key = "test", stuff = "test" };
            var response = await _docClient.PostDocumentAsync("TestCollection", doc1);

            var updateResponse = await _docClient.PutDocumentAsync(
                response._id, 
                new { stuff = "new" });

            Assert.NotNull(response._rev);
            Assert.NotNull(updateResponse._rev);
            Assert.NotEqual(response._rev, updateResponse._rev);
        }

        [Fact]
        public async Task PutDocuments_ShouldSucceed()
        {
            var response = await _docClient.PostDocumentsAsync("TestCollection",
                new[] {
                    new { value = 1 },
                    new { value = 2 }
                });

            var updateResponse = await _docClient.PutDocumentsAsync("TestCollection",
                new[]
                {
                    new { response[0]._key, value = 3 },
                    new { response[1]._key, value = 4 }
                });

            Assert.NotNull(response[0]._rev);
            Assert.NotNull(updateResponse[0]._rev);
            Assert.NotEqual(response[0]._rev, updateResponse[0]._rev);

            Assert.NotNull(response[1]._rev);
            Assert.NotNull(updateResponse[1]._rev);
            Assert.NotEqual(response[1]._rev, updateResponse[1]._rev);
        }
    }
}
