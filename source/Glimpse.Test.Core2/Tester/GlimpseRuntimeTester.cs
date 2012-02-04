using System.Collections.Generic;
using System.Diagnostics;
using Glimpse.Core2;
using Glimpse.Core2.Extensibility;
using Glimpse.Core2.Framework;
using Glimpse.Test.Core2.Extensions;
using Moq;
using System;

namespace Glimpse.Test.Core2.Tester
{
    public class GlimpseRuntimeTester : GlimpseRuntime
    {
        public Mock<IFrameworkProvider> FrameworkProviderMock { get; set; }
        public Mock<ResourceEndpointConfiguration> EndpointConfigMock { get; set; }
        public Mock<IDataStore> HttpRequestStoreMock { get; set; }
        public Mock<ITabMetadata> TabMetadataMock { get; set; }
        public Mock<ITab> TabMock { get; set; }
        public Mock<IPipelineInspector> PipelineInspectorMock { get; set; }
        public Mock<ISerializer> SerializerMock { get; set; }
        public Mock<IPersistanceStore> PersistanceStoreMock { get; set; }
        public Mock<ILogger> LoggerMock { get; set; }
        public Mock<IRequestMetadata> RequestMetadataMock { get; set; }
        public Mock<IResource> ResourceMock { get; set; }
        public Mock<IResourceResult> ResourceResultMock { get; set; }
        public Mock<IRuntimePolicy> ValidatorMock { get; set; }
        public GlimpseConfiguration Configuration { get; set; }
        public Mock<IStaticClientScript> StaticScriptMock { get; set; }
        public Mock<IDynamicClientScript> DynamicScriptMock { get; set; }
        public Mock<IHtmlEncoder> EncoderMock { get; set; }

        private GlimpseRuntimeTester(GlimpseConfiguration configuration, Mock<IFrameworkProvider> frameworkProviderMock, Mock<ResourceEndpointConfiguration> endpointConfigMock) : base(configuration)
        {
            FrameworkProviderMock = frameworkProviderMock;
            EndpointConfigMock = endpointConfigMock;
            HttpRequestStoreMock = new Mock<IDataStore>();
            TabMetadataMock = new Mock<ITabMetadata>().Setup();
            TabMock = new Mock<ITab>().Setup();
            PipelineInspectorMock = new Mock<IPipelineInspector>();
            SerializerMock = new Mock<ISerializer>();
            PersistanceStoreMock = new Mock<IPersistanceStore>();
            LoggerMock = new Mock<ILogger>();
            ResourceMock = new Mock<IResource>();
            ResourceResultMock = new Mock<IResourceResult>();
            ValidatorMock = new Mock<IRuntimePolicy>();
            ValidatorMock.Setup(v => v.Execute(It.IsAny<IRuntimePolicyContext>())).Returns(RuntimePolicy.On);
            RequestMetadataMock = new Mock<IRequestMetadata>();
            RequestMetadataMock.Setup(r => r.RequestHttpMethod).Returns("GET");
            RequestMetadataMock.Setup(r => r.RequestIsAjax).Returns(true);
            RequestMetadataMock.Setup(r => r.RequestUri).Returns("http://localhost");
            RequestMetadataMock.Setup(r => r.ResponseStatusCode).Returns(200);
            RequestMetadataMock.Setup(r => r.ResponseContentType).Returns(@"text\html");
            RequestMetadataMock.Setup(r => r.GetCookie(Constants.ClientIdCookieName)).Returns(@"Some Client");
            RequestMetadataMock.Setup(r => r.GetCookie(Constants.UserAgentHeaderName)).Returns(@"FireFox");
            RequestMetadataMock.Setup(r => r.GetHttpHeader(Constants.HttpRequestHeader)).Returns(Guid.NewGuid().ToString());
            StaticScriptMock = new Mock<IStaticClientScript>();
            StaticScriptMock.Setup(ss => ss.Order).Returns(ScriptOrder.ClientInterfaceScript);
            StaticScriptMock.Setup(ss => ss.GetUri(Version)).Returns("http://localhost/static");
            DynamicScriptMock = new Mock<IDynamicClientScript>();
            DynamicScriptMock.Setup(ds => ds.Order).Returns(ScriptOrder.RequestDataScript);
            DynamicScriptMock.Setup(ds => ds.GetResourceName()).Returns("aResource");
            EncoderMock = new Mock<IHtmlEncoder>();


            FrameworkProviderMock.Setup(fp => fp.RequestMetadata).Returns(RequestMetadataMock.Object);

            configuration.Serializer = SerializerMock.Object;
            configuration.PersistanceStore = PersistanceStoreMock.Object;
            configuration.Logger = LoggerMock.Object;
            configuration.BasePolicy = RuntimePolicy.On;
            configuration.HtmlEncoder = EncoderMock.Object;

            Configuration = configuration;
        }

        public static GlimpseRuntimeTester Create()
        {
            var frameworkProviderMock = new Mock<IFrameworkProvider>().Setup();
            var endpointConfigMock = new Mock<ResourceEndpointConfiguration>();
            var clientScripts = new List<IClientScript>();

            var configuration =
                new GlimpseConfiguration(frameworkProviderMock.Object, endpointConfigMock.Object, clientScripts).
                    TurnOffAutoDiscover();


            return new GlimpseRuntimeTester(configuration, frameworkProviderMock, endpointConfigMock);
        }
    }
}