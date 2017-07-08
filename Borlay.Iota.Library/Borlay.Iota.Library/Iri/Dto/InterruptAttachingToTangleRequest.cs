namespace Borlay.Iota.Library.Iri.Dto
{
    /// <summary>
    /// This class represents the core api request 'InterruptAttachingToTangle'
    /// </summary>
    public class InterruptAttachingToTangleRequest : IriRequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterruptAttachingToTangleRequest"/> class.
        /// </summary>
        public InterruptAttachingToTangleRequest() : base(CommandConstants.InterruptAttachingToTangle)
        {
        }
    }
}