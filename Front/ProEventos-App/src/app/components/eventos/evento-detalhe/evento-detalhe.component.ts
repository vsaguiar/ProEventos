import { Component, OnInit, TemplateRef } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { EventoService } from '@app/services/evento.service';
import { LoteService } from './../../../services/lote.service';
import { Evento } from '@app/models/Evento';

import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { Lote } from '@app/models/Lote';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { environment } from '@environments/environment';

@Component({
  selector: 'app-evento-detalhe',
  templateUrl: './evento-detalhe.component.html',
  styleUrls: ['./evento-detalhe.component.scss']
})
export class EventoDetalheComponent implements OnInit {
  modalRef: BsModalRef;
  eventoId: number;
  evento = {} as Evento;
  form!: FormGroup;
  estadoSalvar = 'post';
  loteAtual = { id: 0, nome: '', indice: 0 };
  imagemURL = 'assets/upload.png';
  file: File;

  get modoEditar(): boolean {
    return this.estadoSalvar == 'put';
  }

  get lotes(): FormArray {
    return this.form.get('lotes') as FormArray;
  }

  get f(): any {
    return this.form.controls;
  }

  public bsConfig(): any {
    return {
      adaptivePosition: true,
      dateInputFormat: 'DD/MM/YYYY hh:mm a',
      containerClass: 'theme-default',
      showWeekNumbers: false
    };
  }

  constructor(
    private fb: FormBuilder,
    private localeService: BsLocaleService,
    private activatedRoute: ActivatedRoute,
    private eventoService: EventoService,
    private spinner: NgxSpinnerService,
    private toastr: ToastrService,
    private router: Router,
    private modalService: BsModalService,
    private loteService: LoteService) {
    this.localeService.use('pt-br');
  }

  public carregarEvento(): void {
    this.eventoId = +this.activatedRoute.snapshot.paramMap.get('id');

    if (this.eventoId != null && this.eventoId != 0) {
      this.spinner.show();

      this.estadoSalvar = 'put';

      this.eventoService
        .getEventoById(this.eventoId)
        .subscribe(
          (evento: Evento) => {
            this.evento = { ...evento };
            this.form.patchValue(this.evento);
            if (this.evento.imagemURL != '') {
              this.imagemURL = environment.apiURL + 'resources/images/' + this.evento.imagemURL;
            }
            this.carregarLotes();
          },
          (error: any) => {
            this.toastr.error('Erro ao tentar carregar Evento.', 'Erro');
            console.error(error);
          },
        ).add(() => this.spinner.hide());
    }
  }

  public carregarLotes(): void {
    this.loteService
      .getLotesByEventoId(this.eventoId)
      .subscribe(
        (lotesRetorno: Lote[]) => {
          lotesRetorno.forEach((lote) => {
            this.lotes.push(this.criarLote(lote));
          });
        },
        (error: any) => {
          this.toastr.error('Erro ao tentar carregar lotes.', 'Erro');
          console.error(error);
        }
      )
      .add(() => this.spinner.hide());
  }

  ngOnInit(): void {
    this.carregarEvento();
    this.validation();
  }

  public validation(): void {
    this.form = this.fb.group({
      tema: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(50)]],
      local: ['', Validators.required],
      dataEvento: ['', Validators.required],
      quantidadePessoas: ['', [Validators.required, Validators.max(120000)]],
      telefone: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      imagemURL: [''],
      lotes: this.fb.array([])
    });
  }

  adicionarLote(): void {
    this.lotes.push(this.criarLote({ id: 0 } as Lote));
  }

  criarLote(lote: Lote): FormGroup {
    return this.fb.group({
      id: [lote.id],
      nome: [lote.nome, Validators.required],
      quantidade: [lote.quantidade, Validators.required],
      preco: [lote.preco, Validators.required],
      dataInicio: [lote.dataInicio],
      dataFim: [lote.dataFim]
    });
  }

  public mudarValorData(value: Date, indice: number, campo: string): void {
    this.lotes.value[indice][campo] = value;
  }

  public retornaTituloLote(nome: string): string {
    return nome === null || nome === ''
      ? 'Nome do lote'
      : nome;
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.toastr.error('Dados inválidos.', 'Erro');
      this.spinner.hide();
      return;
    }
  }

  public resetForm(event: any): void {
    event.preventDefault();
    this.form.reset();
  }

  public cssValidator(campoForm: FormControl | AbstractControl): any {
    return { 'is-invalid': campoForm.errors && campoForm.touched }
  }

  public salvarEvento(): void {
    if (this.form.valid) {
      this.spinner.show();
      this.evento = (this.estadoSalvar == 'post')
        ? { ...this.form.value }
        : { id: this.evento.id, ...this.form.value };

      this.eventoService[this.estadoSalvar](this.evento).subscribe(
        (eventoRetorno: Evento) => {
          this.toastr.success('Evento salvo com sucesso!', 'Sucesso');
          this.router.navigate([`eventos/detalhe/${eventoRetorno.id}`]);
        },
        (error: any) => {
          console.error(error);
          this.spinner.hide();
          this.toastr.error('Erro ao salvar o evento.', 'Erro');
        },
        () => this.spinner.hide()
      );
    }
  }

  public salvarLote(): void {
    if (this.form.controls.lotes.valid) {
      this.spinner.show();
      this.loteService.SaveLote(this.eventoId, this.form.value.lotes).subscribe(
        () => {
          this.toastr.success('Lote(s) salvo(s) com sucesso!', 'Sucesso');
          // this.lotes.reset();
        },
        (error: any) => {
          this.toastr.error('Erro ao tentar salvar o(s) lote(s).', 'Erro');
          console.error(error);
        }
      ).add(() => this.spinner.hide());
    }
  }

  public removerLote(template: TemplateRef<any>, indice: number): void {
    this.loteAtual.id = this.lotes.get(indice + '.id').value;
    this.loteAtual.nome = this.lotes.get(indice + '.nome').value;
    this.loteAtual.indice = indice;

    this.modalRef = this.modalService.show(template, { class: 'modal-sm' });
  }

  confirmDeleteLote(): void {
    this.modalRef.hide();
    this.spinner.show();
    this.loteService.delete(this.eventoId, this.loteAtual.id).subscribe(
      () => {
        this.toastr.success('Lote excluído com sucesso!', 'Sucesso');
        this.lotes.removeAt(this.loteAtual.indice);
      },
      (error: any) => {
        this.toastr.error(`Erro ao tentar excluir o lote ${this.loteAtual.id}.`, 'Erro');
        console.error(error);
      }
    ).add(() => this.spinner.hide());
  }

  declineDeleteLote(): void {
    this.modalRef.hide();
  }

  onFileChange(ev: any): void {
    const reader = new FileReader();

    reader.onload = (event: any) => this.imagemURL = event.target.result;

    this.file = ev.target.files;
    reader.readAsDataURL(this.file[0]);

    this.uploadImagem();
  }

  uploadImagem(): void {
    this.spinner.show();
    this.eventoService.postUpload(this.eventoId, this.file).subscribe(
      () => {
        this.carregarEvento();
        this.toastr.success('Imagem atualizada com sucesso!', 'Sucesso');
      },
      (error: any) => {
        this.toastr.error('Não foi possível atualizar a imagem.', 'Erro');
        console.log(error);
      },
    ).add(() => this.spinner.hide());
  }

}
