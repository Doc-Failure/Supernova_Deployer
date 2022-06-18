export const idlFactory = ({ IDL }) => {
  const LogoResult = IDL.Record({ 'data' : IDL.Text, 'logo_type' : IDL.Text });
  const InitArgs = IDL.Record({
    'logo' : IDL.Opt(LogoResult),
    'name' : IDL.Text,
    'custodians' : IDL.Opt(IDL.Vec(IDL.Principal)),
    'symbol' : IDL.Text,
  });
  const ApiError = IDL.Variant({
    'ZeroAddress' : IDL.Null,
    'InvalidTokenId' : IDL.Null,
    'Unauthorized' : IDL.Null,
    'Other' : IDL.Null,
  });
  const TxReceipt = IDL.Variant({ 'Ok' : IDL.Nat, 'Err' : ApiError });
  const MetadataVal = IDL.Variant({
    'Nat64Content' : IDL.Nat64,
    'Nat32Content' : IDL.Nat32,
    'Nat8Content' : IDL.Nat8,
    'NatContent' : IDL.Nat,
    'Nat16Content' : IDL.Nat16,
    'BlobContent' : IDL.Vec(IDL.Nat8),
    'TextContent' : IDL.Text,
  });
  const MetadataKeyVal = IDL.Tuple(IDL.Text, MetadataVal);
  const MetadataPurpose = IDL.Variant({
    'Preview' : IDL.Null,
    'Rendered' : IDL.Null,
  });
  const MetadataPart = IDL.Record({
    'data' : IDL.Vec(IDL.Nat8),
    'key_val_data' : IDL.Vec(MetadataKeyVal),
    'purpose' : MetadataPurpose,
  });
  const MetadataDesc = IDL.Vec(MetadataPart);
  const MetadataResult = IDL.Variant({ 'Ok' : MetadataDesc, 'Err' : ApiError });
  const ExtendedMetadataResult = IDL.Record({
    'token_id' : IDL.Nat64,
    'metadata_desc' : MetadataDesc,
  });
  const HttpRequest = IDL.Record({
    'url' : IDL.Text,
    'method' : IDL.Text,
    'body' : IDL.Vec(IDL.Nat8),
    'headers' : IDL.Vec(IDL.Tuple(IDL.Text, IDL.Text)),
  });
  const HttpResponse = IDL.Record({
    'body' : IDL.Vec(IDL.Nat8),
    'headers' : IDL.Vec(IDL.Tuple(IDL.Text, IDL.Text)),
    'status_code' : IDL.Nat16,
  });
  const MintReceipt = IDL.Variant({
    'Ok' : IDL.Record({ 'id' : IDL.Nat, 'token_id' : IDL.Nat64 }),
    'Err' : IDL.Variant({ 'Unauthorized' : IDL.Null }),
  });
  const OwnerResult = IDL.Variant({ 'Ok' : IDL.Principal, 'Err' : ApiError });
  const ManageResult = IDL.Variant({ 'Ok' : IDL.Null, 'Err' : ApiError });
  const InterfaceId = IDL.Variant({
    'Burn' : IDL.Null,
    'Mint' : IDL.Null,
    'Approval' : IDL.Null,
    'TransactionHistory' : IDL.Null,
    'TransferNotification' : IDL.Null,
  });
  return IDL.Service({
    'approveDip721' : IDL.Func([IDL.Principal, IDL.Nat64], [TxReceipt], []),
    'balanceOfDip721' : IDL.Func([IDL.Principal], [IDL.Nat64], ['query']),
    'burnDip721' : IDL.Func([IDL.Nat64], [TxReceipt], []),
    'getApprovedDip721' : IDL.Func([IDL.Nat64], [TxReceipt], ['query']),
    'getMetadataDip721' : IDL.Func([IDL.Nat64], [MetadataResult], ['query']),
    'getMetdataForUserDip721' : IDL.Func(
        [IDL.Principal],
        [IDL.Vec(ExtendedMetadataResult)],
        [],
      ),
    'http_request' : IDL.Func([HttpRequest], [HttpResponse], ['query']),
    'isApprovedForAllDip721' : IDL.Func([IDL.Principal], [IDL.Bool], ['query']),
    'is_custodian' : IDL.Func([IDL.Principal], [IDL.Bool], ['query']),
    'logoDip721' : IDL.Func([], [LogoResult], ['query']),
    'mintDip721' : IDL.Func(
        [IDL.Principal, MetadataDesc, IDL.Vec(IDL.Nat8)],
        [MintReceipt],
        [],
      ),
    'nameDip721' : IDL.Func([], [IDL.Text], ['query']),
    'ownerOfDip721' : IDL.Func([IDL.Nat64], [OwnerResult], ['query']),
    'safeTransferFromDip721' : IDL.Func(
        [IDL.Principal, IDL.Principal, IDL.Nat64],
        [TxReceipt],
        [],
      ),
    'safeTransferFromNotifyDip721' : IDL.Func(
        [IDL.Principal, IDL.Principal, IDL.Nat64, IDL.Vec(IDL.Nat8)],
        [TxReceipt],
        [],
      ),
    'setApprovalForAllDip721' : IDL.Func(
        [IDL.Principal, IDL.Bool],
        [TxReceipt],
        [],
      ),
    'set_custodian' : IDL.Func([IDL.Principal, IDL.Bool], [ManageResult], []),
    'set_logo' : IDL.Func([IDL.Opt(LogoResult)], [ManageResult], []),
    'set_name' : IDL.Func([IDL.Text], [ManageResult], []),
    'set_symbol' : IDL.Func([IDL.Text], [ManageResult], []),
    'supportedInterfacesDip721' : IDL.Func(
        [],
        [IDL.Vec(InterfaceId)],
        ['query'],
      ),
    'symbolDip721' : IDL.Func([], [IDL.Text], ['query']),
    'totalSupplyDip721' : IDL.Func([], [IDL.Nat64], ['query']),
    'transferFromDip721' : IDL.Func(
        [IDL.Principal, IDL.Principal, IDL.Nat64],
        [TxReceipt],
        [],
      ),
    'transferFromNotifyDip721' : IDL.Func(
        [IDL.Principal, IDL.Principal, IDL.Nat64, IDL.Vec(IDL.Nat8)],
        [TxReceipt],
        [],
      ),
  });
};
export const init = ({ IDL }) => {
  const LogoResult = IDL.Record({ 'data' : IDL.Text, 'logo_type' : IDL.Text });
  const InitArgs = IDL.Record({
    'logo' : IDL.Opt(LogoResult),
    'name' : IDL.Text,
    'custodians' : IDL.Opt(IDL.Vec(IDL.Principal)),
    'symbol' : IDL.Text,
  });
  return [InitArgs];
};
