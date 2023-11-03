namespace KrMicro.Core.CQS.Command.Jwt;

public record GenerateJwtCommandRequest(string UserName, string Role);