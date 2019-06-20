/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

 /* global getAssetRegistry getFactory emit */

/**
 * UpdateContract transaction processor function.
 * @param {org.example.basic.UpdateContract} tx The UpdateContract transaction instance.
 * @transaction
 */
async function updateContract(tx) {
    console.log('updateContract');

	const factory = getFactory();
	const contract = tx.contract;
	const newData = tx.newData;

	/*
		// Can trigger multiple events (i.e. one per field modified, instead of having one at the end.
        for (let x = 0; x < vehiclesToScrap.length; x++) {
            vehiclesToScrap[x].vehicleStatus = 'SCRAPPED';
            const scrapVehicleEvent = factory.newEvent(NS_D, 'ScrapVehicleEvent');
            scrapVehicleEvent.vehicle = vehiclesToScrap[x];
            emit(scrapVehicleEvent);
        }
	*/
    // Update the contract with the new values.
	if (newData.expiryDate){
		contract.expiryDate = newData.expiryDate;
	}
	if (newData.durationDays){
		contract.durationDays = newData.durationDays;
	}
	if (newData.status){
		contract.status = newData.status;
	}
	if (newData.content){
		contract.content = newData.content;
	}

    // Get the asset registry for the asset.
    const assetRegistry = await getAssetRegistry(contract.getFullyQualifiedType());
    // Update the asset in the asset registry.
    await assetRegistry.update(contract);

    // Emit an event for the modified asset.
    let event = factory.newEvent('org.example.basic', 'UpdateContractEvent');
    event.contract = contract;
    event.newData = newData;
    emit(event);

}