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

namespace OxfordDictExtractor.Core
{
    // See https://www.wordfrequency.info/files.asp
    // Rank: 1-62,000. Based on word frequency.
    // Lemma: Again, the "dictionary / headword" entry. This is why was or happier or shoes would
    //        not be included here; they are word forms of the lemmas be, happy, and shoe.
    // PoS: Part of speech. This is the first letter of the codes from https://ucrel.lancs.ac.uk/claws7tags.html
    //      (e.g. N for noun, V for verb, A for adjective, R for adverb, P for preposition, C for conjunction,
    //      D for determiner, I for interjection, M for numeral, U for pronoun, X for other).
    internal record CocaWordFrequencyLemmaEntry(int Rank, string Lemma, string PoS)
    {
    }
}
