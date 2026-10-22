import { useEffect, useRef, useState } from 'react';
type Curso = { id:number; nomeCompleto:string; nomeBreve:string; descricao:string; categoria:string; modalidade:string; cargaHoraria:number; publicoAlvo:string; status:string; imagemUrl:string; dataDisponibilizacao:string|null };
type Aula = { id:number; titulo:string; descricao:string; ordem:number };
type Modulo = { id:number; titulo:string; ordem:number; aulas:Aula[] };
type CursoDetalhe = Curso & { modulos:Modulo[] };
type Filtros = { categorias:string[]; modalidades:string[]; status:string[] };
// Em produção, o ASP.NET Core serve o frontend e a API na mesma origem.
const API = import.meta.env.VITE_API_URL ?? (import.meta.env.PROD ? '' : 'http://localhost:5080');
export default function App() {
 const [cursos,setCursos] = useState<Curso[]>([]);
 const [filtros,setFiltros] = useState<Filtros>({categorias:[],modalidades:[],status:[]});
 const [pesquisa,setPesquisa] = useState(''); const [categoria,setCategoria] = useState('');
 const [modalidade,setModalidade] = useState(''); const [status,setStatus] = useState('');
 const [carregando,setCarregando] = useState(true);const [erro,setErro] = useState('');
 const [detalhe,setDetalhe] = useState<CursoDetalhe|null>(null);
 const [cursoAbertoId,setCursoAbertoId] = useState<number|null>(null);
 const [carregandoDetalhe,setCarregandoDetalhe] = useState(false);
 const [erroDetalhe,setErroDetalhe] = useState('');
 const detalheRequisicao = useRef<AbortController|null>(null);
 useEffect(()=>{fetch(`${API}/api/filtros`).then(r=>{if(!r.ok) throw Error(); return r.json()}).then(setFiltros).catch(()=>setErro('Não foi possível carregar os filtros.'))},[]);
 useEffect(()=>{const controller=new AbortController();const timeout=setTimeout(async()=>{
  setCarregando(true);setErro('');
  detalheRequisicao.current?.abort();setCursoAbertoId(null);setDetalhe(null);
  setCarregandoDetalhe(false);setErroDetalhe('');
  const q=new URLSearchParams();if(pesquisa.trim())q.set('pesquisa',pesquisa.trim());if(categoria)q.set('categoria',categoria);if(modalidade)q.set('modalidade',modalidade);if(status)q.set('status',status);
  try {const r=await fetch(`${API}/api/cursos?${q}`,{signal:controller.signal});if(!r.ok)throw Error();const dados=await r.json();setCursos(dados.itens)}
  catch(e){if(!controller.signal.aborted)setErro('Não foi possível consultar os cursos. Verifique se a API está ativa.')}finally{if(!controller.signal.aborted)setCarregando(false)}
 },250);return()=>{clearTimeout(timeout);controller.abort()};},[pesquisa,categoria,modalidade,status]);
 const alternarDetalhe=async(id:number)=>{
   detalheRequisicao.current?.abort();
   if(cursoAbertoId===id){setCursoAbertoId(null);setDetalhe(null);setCarregandoDetalhe(false);setErroDetalhe('');return}
   const controller=new AbortController();detalheRequisicao.current=controller;
   setCursoAbertoId(id);setDetalhe(null);setCarregandoDetalhe(true);setErroDetalhe('');
   try{const r=await fetch(`${API}/api/cursos/${id}`,{signal:controller.signal});if(!r.ok)throw Error();
     const dados=await r.json() as CursoDetalhe;if(!controller.signal.aborted)setDetalhe(dados)
   }catch{if(!controller.signal.aborted)setErroDetalhe('Falha ao carregar os detalhes deste curso.')}
   finally{if(!controller.signal.aborted)setCarregandoDetalhe(false)}
 };
 useEffect(()=>()=>detalheRequisicao.current?.abort(),[]);
 const limpar=()=>{setPesquisa('');setCategoria('');setModalidade('');setStatus('')};
 const filtrosAtivos=[
   ...(pesquisa.trim()?[{chave:'pesquisa',rotulo:`Nome: ${pesquisa.trim()}`,remover:()=>setPesquisa('')}]:[]),
   ...(categoria?[{chave:'categoria',rotulo:`Categoria: ${categoria}`,remover:()=>setCategoria('')}]:[]),
   ...(modalidade?[{chave:'modalidade',rotulo:`Modalidade: ${modalidade}`,remover:()=>setModalidade('')}]:[]),
   ...(status?[{chave:'status',rotulo:`Status: ${status}`,remover:()=>setStatus('')}]:[])
 ];
 return <><header><img src="/assets/logo-sesi-industria-primary.svg" alt="SESI Indústria"/><span>Catálogo de cursos • protótipo técnico</span></header><main>
  <h1>Explore nossos cursos</h1><p>Encontre formações por área, modalidade ou disponibilidade.</p>
  <section aria-label="Pesquisa e filtros" className="filtros"><label>Pesquisar curso<input placeholder="Nome do curso" value={pesquisa} onChange={e=>setPesquisa(e.target.value)}/></label>
  {([['Categoria',categoria,setCategoria,filtros.categorias],['Modalidade',modalidade,setModalidade,filtros.modalidades],['Status',status,setStatus,filtros.status]] as const).map(([rotulo,valor,set,opcoes])=><label key={rotulo}>{rotulo}<select value={valor} onChange={e=>set(e.target.value)}><option value="">Todas</option>{opcoes.map(x=><option key={x}>{x}</option>)}</select></label>)}
  <button type="button" onClick={limpar}>Limpar filtros</button></section>
  <p className="ajuda-filtros">A pesquisa busca pelo nome completo ou breve do curso. Para listar todos os cursos de uma área, selecione a categoria sem preencher a pesquisa. Os filtros são combinados.</p>
  {filtrosAtivos.length>0&&<section className="filtros-ativos" aria-label="Filtros ativos"><strong>Filtros ativos:</strong>{filtrosAtivos.map(f=><button key={f.chave} type="button" className="filtro-chip" aria-label={`Remover filtro ${f.rotulo}`} onClick={f.remover}>{f.rotulo} ×</button>)}</section>}
  <div role="status" aria-live="polite">{carregando?'Carregando cursos...':`${cursos.length} curso(s) encontrado(s)`}</div>
  {erro&&<p role="alert" className="erro">{erro}</p>}
  {!carregando&&!erro&&cursos.length===0&&<div className="sem-resultados"><h2>Nenhum curso encontrado</h2><p>Não há cursos que correspondam à combinação de filtros. Remova um filtro ou tente outra pesquisa.</p><button type="button" onClick={limpar}>Limpar todos os filtros</button></div>}
  {!carregando&&!erro&&<section className="grade" aria-label="Resultados">{cursos.map(c=><article key={c.id}><img src={c.imagemUrl} alt=""/><div className="corpo"><small>{c.categoria} • {c.modalidade} • {c.cargaHoraria}h</small><h2>{c.nomeCompleto}</h2><p>{c.descricao}</p><p><strong>Status:</strong> {c.status}</p><button type="button" aria-expanded={cursoAbertoId===c.id} aria-controls={`detalhe-curso-${c.id}`} onClick={()=>void alternarDetalhe(c.id)}>{cursoAbertoId===c.id?'Fechar detalhes':'Ver detalhes'}</button></div>
    {cursoAbertoId===c.id&&<section id={`detalhe-curso-${c.id}`} className="detalhe-inline" aria-label={`Detalhes de ${c.nomeCompleto}`} aria-busy={carregandoDetalhe}>
      <h3>Detalhes do curso</h3>
      {carregandoDetalhe&&<p role="status">Carregando detalhes...</p>}
      {erroDetalhe&&<p role="alert" className="erro">{erroDetalhe}</p>}
      {detalhe?.id===c.id&&<>
        <p><strong>Nome breve:</strong> {detalhe.nomeBreve}</p>
        <p><strong>Modalidade:</strong> {detalhe.modalidade}</p>
        <p><strong>Carga horária:</strong> {detalhe.cargaHoraria} horas</p>
        <p><strong>Público-alvo:</strong> {detalhe.publicoAlvo}</p>
        <p><strong>Disponibilização:</strong> {detalhe.dataDisponibilizacao??'Não informada'}</p>
        <h4>Conteúdo programático</h4>
        {detalhe.modulos.length===0&&<p>Sem módulos cadastrados.</p>}
        {detalhe.modulos.map(m=><div key={m.id} className="modulo-inline"><h5>{m.ordem}. {m.titulo}</h5><ul>{m.aulas.map(a=><li key={a.id}><strong>{a.titulo}:</strong> {a.descricao}</li>)}</ul></div>)}
      </>}
    </section>}
    </article>)}</section>}

 </main><footer>Projeto demonstrativo — dados fictícios</footer></>;
}
