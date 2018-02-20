﻿// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System.Xml;

namespace OsmSharp.IO.Xml
{
    /// <summary>
    /// Reprents an xml source.
    /// </summary>
    public interface IXmlSource
    {
        /// <summary>
        /// Returns true if the xml source is readonly.
        /// </summary>
        bool IsReadOnly
        {
            get;
        }

        /// <summary>
        /// Returns true if the source has data.
        /// </summary>
        bool HasData
        {
            get;
        }

        /// <summary>
        /// Returns the reader for this xml source.
        /// </summary>
        XmlReader GetReader();

        /// <summary>
        /// Returns a write for this xml source.
        /// </summary>
        XmlWriter GetWriter();

        /// <summary>
        /// Closes the xml source.
        /// </summary>
        void Close();
    }
}