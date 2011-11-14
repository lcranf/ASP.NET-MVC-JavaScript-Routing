using System;
using System.Linq;
using System.Web.Mvc;
using JsRouting.Core;
using StructureMap;
using Xunit;
using System.Collections.Generic;

namespace Test
{
    public class ControllerReaderTests
    {
        [Fact]
        public void Ctr_ThrowsException_GivenNullContainer()
        {
            Assert.Throws<ArgumentNullException>(() => new ControllerReader(null, new List<IControllerActionInterceptor>()));           
        }

        [Fact]
        public void Ctr_AddsControllerActions_FromContainerModel()
        {
            var reader = new ControllerReader(new [] { typeof(TestController), typeof(TestController2) }, new IControllerActionInterceptor[0]);
            Assert.Equal(2, reader.Definitions.Count());
        }

        [Fact]
        public void Ctr_DoesNotAddController_WhenInterceptorReturnsFalse()
        {
            var types = new []
            {
                typeof(TestController),
                typeof(TestController2),
            };

            var reader = new ControllerReader(types, new[] { new BlankInterceptor { ReturnValue = true }, new BlankInterceptor { ReturnValue = false }, new BlankInterceptor { ReturnValue = true } });
            Assert.Equal(0, reader.Definitions.Count());
        }

        [Fact]
        public void Ctr_AddsControllerParameters()
        {
            var types = new[]
            {
                typeof(TestController3),
            };

            var reader = new ControllerReader(types, new IControllerActionInterceptor[0]);
            Assert.NotEmpty(reader.Definitions.First(a => a.ActionName.Equals("ParamTest")).Parameters);
        }

        [Fact]
        public void Ctr_ControllerActionParameters_WithCorrectName()
        {
            var types = new[]
            {
                typeof(TestController3),
            };

            var reader = new ControllerReader(types, new IControllerActionInterceptor[0]);
            Assert.Equal("id", reader.Definitions.First(a => a.ActionName.Equals("ParamTest")).Parameters.First.Value.Name);
        }

        [Fact]
        public void Ctr_ControllerActionParameters_SetToOptionalIfDefaultValueSpecified()
        {
            var types = new[]
            {
                typeof(TestController3),
            };

            var reader = new ControllerReader(types, new IControllerActionInterceptor[0]);
            Assert.True(reader.Definitions.First(a => a.ActionName.Equals("DefaultTest")).Parameters.First.Value.Optional);
        }

        [Fact]
        public void Ctr_ControllerActionParameters_DefaultValueSetToDefaultVAlueIfSpecified()
        {
            var types = new[]
            {
                typeof(TestController3),
            };

            var reader = new ControllerReader(types, new IControllerActionInterceptor[0]);
            Assert.Equal("\"" + TestController3.defaultValue + "\"", reader.Definitions.First(a => a.ActionName.Equals("DefaultTest")).Parameters.First.Value.DefaultValue);
        }

        [Fact]
        public void Ctr_ControllerActionParameters_SetsDescription_IfDocumentationSpecified()
        {
            var types = new[]
            {
                typeof(TestController3),
            };

            var reader = new ControllerReader(types, new IControllerActionInterceptor[0]);
            Assert.NotEmpty(reader.Definitions.First(a => a.ActionName.Equals("SummaryTest")).Parameters.First.Value.Description);
        }

        [Fact]
        public void Ctr_ControllerAction_SetsDescription_WhenSummaryXMLContainsSummary()
        {
            var types = new[]
            {
                typeof(TestController3),
            };

            var reader = new ControllerReader(types, new IControllerActionInterceptor[0]);
            Assert.NotEmpty(reader.Definitions.First(a => a.ActionName.Equals("ActionSummaryTest")).Description);
        }

        public class TestController : Controller
        {
            public ActionResult Index()
            {
                return null;
            }
        }

        public class TestController2 : Controller
        {
            public EmptyResult Index()
            {
                return null;
            }
        }

        public class BlankInterceptor : IControllerActionInterceptor
        {
            public bool ReturnValue { get; set; }

            public bool Intercept(ControllerActionDefinition definition)
            {
                return ReturnValue;
            }
        }
    }

    public class TestController3 : Controller
    {
        public const string defaultValue = "default";
        public EmptyResult ParamTest(string id)
        {
            return null;
        }

        public EmptyResult NullableTest(int? id)
        {
            return null;
        }

        public EmptyResult DefaultTest(string id = defaultValue)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Testing the id summary</param>
        /// <returns></returns>
        public EmptyResult SummaryTest(int id)
        {
            return null;
        }

        /// <summary>
        /// Testing the summary
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EmptyResult ActionSummaryTest(int id)
        {
            return null;
        }
    }
}