//----------------------------------------------------------------------------------------------------
// <copyright company="Avira Operations GmbH & Co. KG and its licensors">
// ?2016 Avira Operations GmbH & Co. KG and its licensors.  All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------------------


namespace FileDownloader
{
    internal class CachObject
    {
        public string URI { get; set; }
        public string PATH { get; set; }
        public string[] HEADERS { get; set; }
    }
}