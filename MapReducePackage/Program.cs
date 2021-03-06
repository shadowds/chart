﻿using Microsoft.Hadoop.MapReduce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MapReducePackage
{
    class Program
    {

        static void Main(string[] args)
        {
            var hadoop = Hadoop.Connect();
            var result = hadoop.MapReduceJob.ExecuteJob<NamespaceCounterJob>();
            //- See more at: http://www.amazedsaint.com/2013/03/taming-big-data-with-c-using-hadoop-on.html#sthash.cn9IQRCL.dpuf
        }

        //Mapper 
        public class NamespaceMapper : MapperBase
        {
            //Override the map method. 
            public override void Map(string inputLine, MapperContext context)
            {
                //Extract the namespace declarations in the Csharp files 
                var reg = new Regex(@"(using)\s[A-za-z0-9_\.]*\;");
                var matches = reg.Matches(inputLine);
                foreach (Match match in matches)
                {
                    //Just emit the namespaces. 
                    context.EmitKeyValue(match.Value, "1");
                }
            }
        }

        //Reducer 
        public class NamespaceReducer : ReducerCombinerBase
        {
            //Accepts each key and count the occurrances 
            public override void Reduce(string key, IEnumerable<string> values, ReducerCombinerContext context)
            {
                //Write back 
                context.EmitKeyValue(key, values.Count().ToString());
            }

        }
        //Our Namespace counter job 
        public class NamespaceCounterJob : HadoopJob<NamespaceMapper, NamespaceReducer>
        {
            public override HadoopJobConfiguration Configure(ExecutorContext context)
            {
                var config = new HadoopJobConfiguration();
                config.InputPath = "/input/CodeFiles";
                config.OutputFolder = "/output/CodeFiles";
                return config;
            }
        }
        //- See more at: http://www.amazedsaint.com/2013/03/taming-big-data-with-c-using-hadoop-on.html#sthash.cn9IQRCL.dpuf
    }
}
