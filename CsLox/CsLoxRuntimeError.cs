namespace CsLox
{
    public class CsLoxRuntimeError : Exception {
        public Token Token { get; private set; }
        public CsLoxRuntimeError(Token token, string? message) : base(message)
        {
            this.Token = token;
        }
    }
}
