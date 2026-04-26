using BedrockProtocol;
using PlayStatus = BedrockProtocol.Types.PlayStatus;

namespace Lantern.Utils;

public static class ProtocolSupport {

    public const string BaseGameVersion = "1.21.93";

    public static bool TryValidateClientProtocol(
        int protocolVersion,
        out PlayStatus rejectionStatus,
        out string logMessage
    ) {
        if (Info.SupportedProtocols.Count == 0) {
            rejectionStatus = PlayStatus.LoginFailedServerOld;
            logMessage = "No supported protocol configured on server";
            return false;
        }

        var minSupportedProtocol = Info.SupportedProtocols.Min();
        var maxSupportedProtocol = Info.SupportedProtocols.Max();

        if (protocolVersion < minSupportedProtocol) {
            rejectionStatus = PlayStatus.LoginFailedClientOld;
            logMessage = $"Client protocol version {protocolVersion} is outdated";
            return false;
        }

        if (protocolVersion > maxSupportedProtocol) {
            rejectionStatus = PlayStatus.LoginFailedServerOld;
            logMessage = $"Client protocol version {protocolVersion} is newer than supported by server";
            return false;
        }

        rejectionStatus = PlayStatus.LoginSuccess;
        logMessage = string.Empty;
        return true;
    }
}
