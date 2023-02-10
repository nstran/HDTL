import { Component, Input } from '@angular/core'
import * as _ from 'lodash'

class ErrorDef {
	error: string
	localizationKey: string
	errorProperty: string
}

@Component({
	selector: '<validation-messages>',
	template: `<div class="has-danger" *ngIf="formCtrl.invalid && (formCtrl.dirty || formCtrl.touched)">
		<div *ngFor="let errorDef of errorDefsInternal">
			<div *ngIf="getErrorDefinitionIsInValid(errorDef)" class="text-danger">
				{{ getErrorDefinitionMessage(errorDef) }}
			</div>
		</div>
	</div>`,
})
export class ValidationMessagesComponent {
	_errorDefs: ErrorDef[] = [];

	@Input() formCtrl;
	@Input() customMessage: any[];
	@Input() set errorDefs(value: ErrorDef[]) {
		this._errorDefs = value;
	}

	readonly standartErrorDefs: ErrorDef[] = [
		{ error: 'required', localizationKey: 'ThisFieldIsRequired' } as ErrorDef,
		{ error: 'minlength', localizationKey: 'PleaseEnterAtLeastNCharacter', errorProperty: 'requiredLength' } as ErrorDef,
		{ error: 'maxlength', localizationKey: 'PleaseEnterNoMoreThanNCharacter', errorProperty: 'requiredLength' } as ErrorDef,
		{ error: 'email', localizationKey: 'InvalidEmailAddress' } as ErrorDef,
		{ error: 'pattern', localizationKey: 'InvalidPattern', errorProperty: 'requiredPattern' } as ErrorDef,
		{ error: 'min', localizationKey: 'InvalidMin', errorProperty: 'requiredMin' } as ErrorDef,
		{ error: 'max', localizationKey: 'InvalidMax', errorProperty: 'requiredMax' } as ErrorDef,
	]

	get errorDefsInternal(): ErrorDef[] {
		let standarts = _.filter(this.standartErrorDefs, (ed) => !_.find(this._errorDefs, (edC) => edC.error === ed.error))
		let all = <ErrorDef[]>_.concat(standarts, this._errorDefs)

		return all
	}

	getErrorDefinitionIsInValid(errorDef: ErrorDef): boolean {
		return !!this.formCtrl.errors[errorDef.error]
	}

	getErrorDefinitionMessage(errorDef: ErrorDef): string {
        let errorRequirement = this.formCtrl.errors[errorDef.error][errorDef.errorProperty];
        let localizationKey = errorDef.localizationKey
		if (this.customMessage && this.customMessage.length > 0) {
			let temp = this.customMessage.find((x) => x.error === errorDef.error)
			localizationKey = temp ? temp.message : errorDef.localizationKey
		}
		return !!errorRequirement ? errorRequirement : localizationKey
    }
	

}
