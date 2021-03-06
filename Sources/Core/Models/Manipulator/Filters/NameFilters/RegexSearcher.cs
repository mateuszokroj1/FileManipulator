﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using FileManipulator.Models.Manipulator.FileInfos;

namespace FileManipulator.Models.Manipulator.Filters.NameFilters
{
    public class RegexSearcher : SubTask, INameFilter
    {
        public RegexSearcher(ICollection<IFilter> subTaskCollection)
        {
            this.collection = subTaskCollection ?? throw new ArgumentNullException(nameof(subTaskCollection));
        }

        private const string SimpleName = "NameFilters.RegexSearcher";
        private Regex regex;
        private readonly ICollection<IFilter> collection;

        public Regex Regex
        {
            get => this.regex;
            set => SetProperty(ref this.regex, value);
        }

        public async Task<IEnumerable<ISourceFileInfo>> FilterAsync(IEnumerable<ISourceFileInfo> inputList)
        {
            if (Regex != null)
                return inputList.Where(fileInfo => Regex.IsMatch(fileInfo.SourceFileName));
            else
                return Enumerable.Empty<ISourceFileInfo>();
        }

        public override bool LoadFromSimpleObject(dynamic simpleObject)
        {
            if (simpleObject == null)
                return false;

            if (simpleObject.Type.Value != SimpleName)
                return false;

            if (simpleObject.Parameters == null)
                return false;

            Regex = new Regex(simpleObject.Parameters.Regex.Pattern.Value);

            return true;
        }

        public override object GetSimpleObject()
        {
            return new
            {
                Type = SimpleName,
                Parameters = new
                {
                    Regex = Regex
                }
            };
        }

        public override void Close()
        {
            this.collection.Remove(this);
        }
    }
}
