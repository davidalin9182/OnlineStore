//namespace Proiect_IR.Helpers
//{
//    using Lucene.Net.Analysis;
//    using Lucene.Net.Analysis.Core;
//    using Lucene.Net.Analysis.Standard;
//    using System.IO;

//    public class AnalyzerWithLowercaseFilter : Analyzer
//    {
//        private readonly Analyzer _baseAnalyzer;

//        public AnalyzerWithLowercaseFilter(Analyzer baseAnalyzer)
//        {
//            _baseAnalyzer = baseAnalyzer;
//        }

//        public override TokenStream TokenStream(string fieldName, TextReader reader)
//        {
//            var components = _baseAnalyzer.CreateComponents(fieldName, reader);
//            return new LowercaseFilter(components.TokenStream);
//        }

//    }

//}
