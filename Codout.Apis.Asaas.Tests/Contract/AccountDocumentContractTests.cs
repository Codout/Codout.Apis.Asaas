using System;
using System.Text.Json;
using Codout.Apis.Asaas.Models.MyAccount;
using Codout.Apis.Asaas.Models.MyAccount.Enums;

namespace Codout.Apis.Asaas.Tests.Contract;

/// <summary>
/// Contract tests para os endpoints de Account Document (MyAccountManager).
/// Schemas verificados via MCP em 2026-05-24:
/// - GET /v3/myAccount/documents (envelope {data:[...]} sem paginacao + rejectReasons)
/// - POST /v3/myAccount/documents/{id} (multipart, retorna AccountDocument {id,status})
/// - GET /v3/myAccount/documents/files/{id} (retorna AccountDocument {id,status})
/// - POST /v3/myAccount/documents/files/{id} (multipart, retorna AccountDocument)
/// - DELETE /v3/myAccount/documents/files/{id} ({deleted, id})
/// </summary>
public class AccountDocumentContractTests
{
    // ─────────────────────────────────────────────────────────────
    // GET /v3/myAccount/documents -> AccountDocumentShowResponseDTO
    // Envelope minimalista: { rejectReasons, data: [...] } (sem paginacao!)
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void PendingDocumentsResponse_DeserializesFromOfficialFixture()
    {
        var json = FixtureLoader.Load("AccountDocument/pending-documents-response.json");

        var result = JsonContractAssert.DeserializeFixture<AccountDocumentResponse>(json);

        Assert.Null(result.RejectReasons);
        Assert.Single(result.Data);

        var group = result.Data[0];
        Assert.Equal("172ed152-4fa4-43ad-9b69-39c323e9526c", group.Id);
        Assert.Equal(AccountDocumentGroupStatus.NOT_SENT, group.Status);
        Assert.Equal(AccountDocumentType.MINUTES_OF_CONSTITUTION, group.Type);
        Assert.Equal("Minutes of election of the last board", group.Title);
        Assert.Equal("No description", group.Description);
        Assert.Equal("https://example.com/cadastro.io/8ad196d6cbfcc5d05bfabcbb5c730f6a", group.OnboardingUrl);
        Assert.Equal(new DateTime(2025, 3, 4), group.OnboardingUrlExpirationDate);

        Assert.NotNull(group.Responsible);
        Assert.Equal("John Doe", group.Responsible.Name);
        Assert.Single(group.Responsible.Type);
        Assert.Equal(AccountDocumentResponsibleType.ASSOCIATION, group.Responsible.Type[0]);

        Assert.Single(group.Documents);
        Assert.Equal("8d257732-2220-11ec-b695-b6af4a64184d", group.Documents[0].Id);
        Assert.Equal(AccountDocumentStatus.PENDING, group.Documents[0].Status);
    }

    [Fact]
    public void PendingDocumentsResponse_UsesMinimalEnvelopeWithoutPagination()
    {
        // B-07 regression: o envelope tem APENAS {rejectReasons, data},
        // sem hasMore/totalCount/limit/offset. Usar ResponseList aqui
        // resultaria em propriedades vazias.
        var json = FixtureLoader.Load("AccountDocument/pending-documents-response.json");

        JsonContractAssert.HasRootProperty(json, "data", JsonValueKind.Array);
        JsonContractAssert.HasRootProperty(json, "rejectReasons", JsonValueKind.Null);

        using var doc = JsonDocument.Parse(json);
        Assert.False(doc.RootElement.TryGetProperty("hasMore", out _));
        Assert.False(doc.RootElement.TryGetProperty("totalCount", out _));
        Assert.False(doc.RootElement.TryGetProperty("limit", out _));
        Assert.False(doc.RootElement.TryGetProperty("offset", out _));
    }

    // ─────────────────────────────────────────────────────────────
    // POST/GET /v3/myAccount/documents[/files]/{id} -> AccountDocumentGetResponseDTO
    // B-20f/B-20g regression: ViewDocumentFile e SubmitDocument antes retornavam
    // AccountDocumentFile (com Name/Url ficticios) e AccountDocumentGroup
    // (objeto rico). Schema real retorna apenas {id, status}.
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void DocumentResponse_HasOnlyIdAndStatus()
    {
        var json = FixtureLoader.Load("AccountDocument/document-response.json");

        var result = JsonContractAssert.DeserializeFixture<AccountDocument>(json);

        Assert.Equal("8d257732-2220-11ec-b695-b6af4a64184d", result.Id);
        Assert.Equal(AccountDocumentStatus.PENDING, result.Status);

        // Regressao B-20f: schema NAO retorna name nem url
        using var doc = JsonDocument.Parse(json);
        Assert.False(doc.RootElement.TryGetProperty("name", out _));
        Assert.False(doc.RootElement.TryGetProperty("url", out _));
    }

    [Fact]
    public void DocumentStatus_AllFourValuesDeserialize()
    {
        // Schema AccountDocumentGetResponseDTO.status: NOT_SENT, PENDING, APPROVED, REJECTED
        foreach (var status in new[] { "NOT_SENT", "PENDING", "APPROVED", "REJECTED" })
        {
            var json = $"{{\"id\":\"x\",\"status\":\"{status}\"}}";
            var result = JsonContractAssert.DeserializeFixture<AccountDocument>(json);
            Assert.Equal(status, result.Status.ToString());
        }
    }

    [Fact]
    public void DocumentGroupStatus_AllFiveValuesDeserialize()
    {
        // Schema AccountDocumentGroupResponseDTO.status tem 5 valores
        // (Group ganha IGNORED em relacao ao Document).
        foreach (var status in new[] { "NOT_SENT", "PENDING", "APPROVED", "REJECTED", "IGNORED" })
        {
            var json = $"{{\"id\":\"x\",\"status\":\"{status}\",\"type\":\"CUSTOM\",\"documents\":[]}}";
            var result = JsonContractAssert.DeserializeFixture<AccountDocumentGroup>(json);
            Assert.Equal(status, result.Status.ToString());
        }
    }

    [Fact]
    public void DocumentType_AllTwelveValuesDeserialize()
    {
        var values = new[] {
            "ALLOW_BANK_ACCOUNT_DEPOSIT_STATEMENT", "CUSTOM", "EMANCIPATION_OF_MINORS",
            "ENTREPRENEUR_REQUIREMENT", "IDENTIFICATION_SELFIE", "IDENTIFICATION",
            "INVOICE", "MEI_CERTIFICATE", "MINUTES_OF_CONSTITUTION",
            "MINUTES_OF_ELECTION", "POWER_OF_ATTORNEY", "SOCIAL_CONTRACT"
        };

        foreach (var type in values)
        {
            var json = $"{{\"id\":\"x\",\"status\":\"NOT_SENT\",\"type\":\"{type}\",\"documents\":[]}}";
            var result = JsonContractAssert.DeserializeFixture<AccountDocumentGroup>(json);
            Assert.Equal(type, result.Type.ToString());
        }
    }

    [Fact]
    public void ResponsibleType_AllThirteenValuesDeserialize()
    {
        var values = new[] {
            "ALLOW_BANK_ACCOUNT_DEPOSIT_STATEMENT", "ASAAS_ACCOUNT_OWNER_EMANCIPATION_AGE",
            "ASAAS_ACCOUNT_OWNER", "ASSOCIATION", "BANK_ACCOUNT_OWNER_EMANCIPATION_AGE",
            "BANK_ACCOUNT_OWNER", "CUSTOM", "DIRECTOR", "INDIVIDUAL_COMPANY",
            "LIMITED_COMPANY", "MEI", "PARTNER", "POWER_OF_ATTORNEY"
        };

        foreach (var t in values)
        {
            var json = $"{{\"name\":\"John\",\"type\":[\"{t}\"]}}";
            var result = JsonContractAssert.DeserializeFixture<AccountDocumentResponsible>(json);
            Assert.Single(result.Type);
            Assert.Equal(t, result.Type[0].ToString());
        }
    }

    // ─────────────────────────────────────────────────────────────
    // POST multipart request — B-20h regression
    // Schema oficial: campos sao "documentFile" (binary) e "type" (enum).
    // C# properties devem ser DocumentFile e Type (FirstCharToLower converte).
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void UploadRequest_HasCorrectMultipartFieldNames()
    {
        // Reflexao sobre as propriedades: garantir que C# property names viram
        // os field names esperados pelo schema apos firstCharToLower do BaseManager.
        var props = typeof(UploadAccountDocumentRequest).GetProperties();
        var names = new System.Collections.Generic.HashSet<string>();
        foreach (var p in props)
        {
            var firstLower = char.ToLowerInvariant(p.Name[0]) + p.Name.Substring(1);
            names.Add(firstLower);
        }

        Assert.Contains("documentFile", names);
        Assert.Contains("type", names);

        // Regressao B-20h: nao deve mais existir "file" (antes era File: IAsaasFile)
        // nem "documentType" (antes era DocumentType: string).
        Assert.DoesNotContain("file", names);
        Assert.DoesNotContain("documentType", names);
    }
}
