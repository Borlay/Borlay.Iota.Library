namespace Borlay.Iota.Library.Iri.Dto
{
    /// <summary>
    /// This class represents the core API request 'GetNeighbors'
    /// </summary>
    /// <seealso cref="Borlay.Iota.Library.Iri.Dto.IriRequestBase" />
    public class GetNeighborsRequest : IriRequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetNeighborsRequest"/> class.
        /// </summary>
        public GetNeighborsRequest() : base(CommandConstants.GetNeighbors)
        {
        }
    }
}