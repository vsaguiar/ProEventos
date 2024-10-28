import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PalestranteService } from '@app/services/palestrante.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { debounceTime, map, tap } from 'rxjs/operators';

@Component({
  selector: 'app-palestrante-detalhe',
  templateUrl: './palestrante-detalhe.component.html',
  styleUrls: ['./palestrante-detalhe.component.scss']
})
export class PalestranteDetalheComponent implements OnInit {

  public form!: FormGroup;
  public situacaoDoForm = '';
  public corDaDescricao = '';

  constructor(
    private fb: FormBuilder,
    private palestranteService: PalestranteService,
    private toastr: ToastrService,
    private spinner: NgxSpinnerService
  ) { }

  ngOnInit() {
    this.validation();
    this.verificaForm();
  }

  private validation(): void{
    this.form = this.fb.group({
      miniCurriculo: ['']
    })
  }

  public get f(): any {
    return this.form.controls;
  }

  private verificaForm(): void {
    this.form.valueChanges
        .pipe(
          map(() => {
            this.situacaoDoForm = 'Mini-currículo está sendo Atualizado!';
            this.corDaDescricao = 'text-warning';
          }),
          debounceTime(1000),
          tap(() => this.spinner.show())
        ).subscribe(
          () => {
            this.palestranteService
                .put({... this.form.value})
                .subscribe(
                  () => {
                    this.situacaoDoForm = 'Mini-currículo foi atualizado!';
                    this.corDaDescricao = 'text-success';
                  },
                  () => {
                    this.toastr.error('Erro ao tentar atualizar palestrante', 'Erro');
                  }
                ).add(() => this.spinner.hide())
          });
  }

}
