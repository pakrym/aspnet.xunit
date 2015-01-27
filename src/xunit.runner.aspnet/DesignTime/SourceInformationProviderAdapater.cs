using System;
using Xunit.Abstractions;
using TestHostSourceInformationProvider = Microsoft.Framework.TestAdapter.ISourceInformationProvider;

namespace Xunit.Runner.AspNet
{
    public class SourceInformationProviderAdapater : ISourceInformationProvider
    {
        private readonly TestHostSourceInformationProvider _inner;

        public SourceInformationProviderAdapater(IServiceProvider services)
        {
            _inner = (TestHostSourceInformationProvider)services.GetService(typeof(TestHostSourceInformationProvider));
        }

        public void Dispose()
        {
        }

        public ISourceInformation GetSourceInformation(ITestCase testCase)
        {
            if (_inner == null)
            {
                return null;
            }

            var innerInformation = _inner.GetSourceInformation(
                testCase.TestMethod.TestClass.Class.Name,
                testCase.TestMethod.Method.Name);

            if (innerInformation == null)
            {
                return null;
            }
            else
            {
                return new SourceInformation()
                {
                    FileName = innerInformation.Filename,
                    LineNumber = innerInformation.LineNumber,
                };
            }
        }

        private class SourceInformation : ISourceInformation
        {
            public string FileName
            {
                get;
                set;
            }

            public int? LineNumber
            {
                get;
                set;
            }

            public void Deserialize(IXunitSerializationInfo info)
            {
                FileName = info.GetValue<string>("FileName");
                LineNumber = info.GetValue<int?>("LineNumber");
            }

            public void Serialize(IXunitSerializationInfo info)
            {
                info.AddValue("FileName", FileName, typeof(string));
                info.AddValue("LineNumber", LineNumber, typeof(int?));
            }
        }
    }
}