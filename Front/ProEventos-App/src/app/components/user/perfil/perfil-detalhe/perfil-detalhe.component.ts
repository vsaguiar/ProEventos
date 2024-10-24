import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AbstractControlOptions, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ValidatorField } from '@app/helpers/ValidatorField';
import { UserUpdate } from '@app/models/Identity/UserUpdate';
import { AccountService } from '@app/services/account.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-perfil-detalhe',
  templateUrl: './perfil-detalhe.component.html',
  styleUrls: ['./perfil-detalhe.component.scss']
})
export class PerfilDetalheComponent implements OnInit {

  @Output() changeFormValue = new EventEmitter();
  userUpdate = {} as UserUpdate;
  form!: FormGroup;

  constructor(private fb: FormBuilder,
    public accountService: AccountService,
    private router: Router,
    private toaster: ToastrService,
    private spinner: NgxSpinnerService) { }

  get f(): any {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.validation();
    this.carregarUsuario();
    this.verificaForm();
  }

  private verificaForm(): void {
    this.form.valueChanges
             .subscribe(() => this.changeFormValue.emit({...this.form.value}))
  }

  private carregarUsuario(): void {
    this.spinner.show();
    this.accountService
      .getUser()
      .subscribe(
        (userRetorno: UserUpdate) => {
          console.log(userRetorno);
          this.userUpdate = userRetorno;
          this.form.patchValue(this.userUpdate);
          // this.toaster.success('Usuário carregado!', 'Sucesso');
        },
        (error) => {
          console.error(error);
          this.toaster.error('Usuário não carregado.', 'Erro');
          this.router.navigate(['/dashboard']);
        }
      ).add(() => this.spinner.hide());
  }

  onSubmit(): void {
    this.atualizarUsuario();
  }

  public atualizarUsuario() {
    this.userUpdate = { ...this.form.value };
    this.spinner.show();

    this.accountService
      .updateUser(this.userUpdate)
      .subscribe(
        () => this.toaster.success('Usuário atualizado!', 'Sucesso'),
        (error) => {
          this.toaster.error(error.error);
          console.error(error);
        }
      ).add(() => this.spinner.hide());
  }

  public resetForm(event: any): void {
    event.preventDefault();
    this.form.reset();
  }

  public validation(): void {
    const formOptions: AbstractControlOptions = {
      validators: ValidatorField.MustMatch('password', 'confirmarPassword')
    };

    this.form = this.fb.group({
      userName: [''],
      imagemURL: [''],
      titulo: ['NaoInformado', Validators.required],
      primeiroNome: ['', Validators.required],
      ultimoNome: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', Validators.required],
      funcao: ['NaoInformado', Validators.required],
      descricao: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(80)]],
      password: ['', [Validators.minLength(4), Validators.nullValidator]],
      confirmarPassword: ['', Validators.nullValidator],
    }, formOptions);
  }

}
