namespace Borlay.Iota.Library.Iri.Dto
{
    /// <summary>
    /// Returns information about your node
    /// </summary>
    public class GetNodeInfoRequest : IriRequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetNodeInfoRequest"/> class.
        /// </summary>
        public GetNodeInfoRequest() : base(CommandConstants.GetNodeInfo)
        {
        }
    }
}