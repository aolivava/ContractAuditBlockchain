# NationwideHire Business Network

> This is the Hyperledger Composer Business Network Definition to keep track of the rent contracts created between NationwideHire and their clients, along with contract changes.

This business network defines:

**Participants**
`HubAdmin`
`HubClient`

**Assets**
`RentContract`
`TandC`

**Transaction**
`UpdateContract`

**Event**
`UpdateContractEvent`

A contract is held between a HubAdmin (usually NationwideHire) and a HubClient. There will be a HubClient per Nationwide Hire client company, and it will have readonly access to the contracts, limited to the ones that belong to the client's company.

A contract can be updated using UpdateContract transaction, which will generate an event with the modified data.

To test this Business Network Definition in the **Test** tab:

Create a `HubAdmin` participant:

```
{
  "$class": "org.example.basic.HubAdmin",
  "participantId": "NationwideHire",
  "name": "Nationwide Hire"
}
```

Create a `HubClient` participant:

```
{
  "$class": "org.example.basic.HubClient",
  "participantId": "AlbertoOliva",
  "name": "Alberto Oliva"
}
```

Create a `RentContract` asset:

```
{
  "$class": "org.example.basic.RentContract",
  "contractId": "contractId:1",
  "contractProvider": "resource:org.example.basic.HubAdmin#NationwideHire",
  "contractClient": "resource:org.example.basic.HubClient#AlbertoOliva",
  "expiryDate": "2019-09-10 12:00",
  "durationDays": 300,
  "status": "CREATED",
  "content": "This is the original content of the contract held between Nationwide Hire and Alberto Oliva.",
}
```

Submit a `UpdateContractData` transaction:

```
{
  "$class": "org.example.basic.UpdateContractData",
  "contract": "resource:org.example.basic.RentContract#contractId:1",
  "newData": {
	  "$class": "org.example.basic.UpdateContractData",
	  "expiryDate": "2019-07-15 12:00",
	  "durationDays": 100,
	  "status": "SIGNED",
	  "content": "The contract between Nationwide Hire and Alberto Oliva has been signed and some data has changed."
	}
}
```

After submitting this transaction, it should be seen in the Transaction Registry, with an emitted `UpdateContractEvent`. As a result, the values of `contractId:1` should have changed in the Asset Registry.

Alberto Oliva Varela