// Copyright (c) 2022 Zhang Shuai<zhangshuai.ustc@gmail.com>.
// All rights reserved.
//
// This file is part of OneDotNet.
//
// OneDotNet is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// OneDotNet is distributed in the hope that it will be useful, but WITHOUT ANY
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
// details.
//
// You should have received a copy of the GNU General Public License along with
// OneDotNet. If not, see <https://www.gnu.org/licenses/>.

namespace OxfordLearnersDictionaryProcessor
{
    public record WordMetadata(
        string HeadWord,
        string PartOfSpeech,
        WordMetadataPhonetics Phonetics,
        List<WordMetadataSense> Senses,
        string? WordOrigin);

    public record WordMetadataPhonetics(string Uk, string Us);

    public record WordMetadataSense(
        int SenseNumber,
        string? CefrLevel,
        List<string> Variants,
        List<string> Uses,
        string? Grammar,
        string? Confer,
        string Definition,
        List<string> Examples,
        List<string> ExtraExamples,
        List<string> Topic);

    public record WordExample(string? Confer, string Sentence);
}
