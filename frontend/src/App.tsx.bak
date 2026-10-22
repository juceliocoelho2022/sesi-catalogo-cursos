import { useEffect, useState } from 'react';
type Curso = { id:number; nomeCompleto:string; nomeBreve:string; descricao:string; categoria:string; modalidade:string; cargaHoraria:number; publicoAlvo:string; status:string; imagemUrl:string };
type Aula = { id:number; titulo:string; descricao:string; ordem:number };
type Modulo = { id:number; titulo:string; ordem:number; aulas:Aula[] };
type CursoDetalhe = Curso & { modulos:Modulo[] };
type Filtros = { categorias:string[]; modalidades:string[]; status:string[] };
const API = import.meta.env.VITE_API_URL || 'http://localhost:5080';
export default function App() {
 const [cursos,setCursos] = useState<Curso[]>([]);
 const [filtros,setFiltros] = useState<Filtros>({categorias:[],modalidades:[],status:[]});
 const [pesquisa,setPesquisa] = useState(''); const [categoria,setCategoria] = useState('');
 const [modalidade,setModalidade] = useState(''); const [status,setStatus] = useState('');
 const [carregando,setCarregando] = useState(true);const [erro,setErro] = useState('');
 const [detalhe,setDetalhe] = useState<CursoDetalhe|null>(null);
 useEffect(()=>{fetch(`${API}/api/filtros`).then(r=>{if(!r.ok) throw Error(); return r.json()}).then(setFiltros).catch(()=>setErro('Não foi possível carregar os filtros.'))},[]);
 useEffect(()=>{const controller=new AbortController();const timeout=setTimeout(async()=>{
  setCarregando(true);setErro('');
  const q=new URLSearchParams();if(pesquisa.trim())q.set('pesquisa',pesquisa.trim());if(categoria)q.set('categoria',categoria);if(modalidade)q.set('modalidade',modalidade);if(status)q.set('status',status);
  try {const r=await fetch(`${API}/api/cursos?${q}`,{signal:controller.signal});if(!r.ok)throw Error();const dados=await r.json();setCursos(dados.itens)}
  catch(e){if(!controller.signal.aborted)setErro('Não foi possível consultar os cursos. Verifique se a API está ativa.')}finally{if(!controller.signal.aborted)setCarregando(false)}
 },250);return()=>{clearTimeout(timeout);controller.abort()};},[pesquisa,categoria,modalidade,status]);
 const abrir=async(id:number)=>{setErro('');try{const r=await fetch(`${API}/api/cursos/${id}`);if(!r.ok)throw Error();setDetalhe(await r.json())}catch{setErro('Falha ao abrir os detalhes do curso.')}};
 const limpar=()=>{setPesquisa('');setCategoria('');setModalidade('');setStatus('')};
 return <><header><img src="/assets/logo-sesi-industria-primary.svg" alt="SESI Indústria"/><span>Catálogo de cursos • protótipo técnico</span></header><main>
  <h1>Explore nossos cursos</h1><p>Encontre formações por área, modalidade ou disponibilidade.</p>
  <section aria-label="Pesquisa e filtros" className="filtros"><label>Pesquisar curso<input placeholder="Nome do curso" value={pesquisa} onChange={e=>setPesquisa(e.target.value)}/></label>
  {([['Categoria',categoria,setCategoria,filtros.categorias],['Modalidade',modalidade,setModalidade,filtros.modalidades],['Status',status,setStatus,filtros.status]] as const).map(([rotulo,valor,set,opcoes])=><label key={rotulo}>{rotulo}<select value={valor} onChange={e=>set(e.target.value)}><option value="">Todas</option>{opcoes.map(x=><option key={x}>{x}</option>)}</select></label>)}
  <button type="button" onClick={limpar}>Limpar filtros</button></section>
  <div role="status" aria-live="polite">{carregando?'Carregando cursos...':`${cursos.length} curso(s) encontrado(s)`}</div>
  {erro&&<p role="alert" className="erro">{erro}</p>}
  {!carregando&&!erro&&cursos.length===0&&<p>Nenhum curso encontrado. Ajuste os filtros.</p>}
  {!carregando&&!erro&&<section className="grade" aria-label="Resultados">{cursos.map(c=><article key={c.id}><img src={c.imagemUrl} alt=""/><div className="corpo"><small>{c.categoria} • {c.modalidade} • {c.cargaHoraria}h</small><h2>{c.nomeCompleto}</h2><p>{c.descricao}</p><p><strong>Status:</strong> {c.status}</p><button onClick={()=>abrir(c.id)}>Ver detalhes</button></div></article>)}</section>}
  {detalhe&&<section className="detalhe" aria-label="Detalhes do curso"><button onClick={()=>setDetalhe(null)}>Fechar detalhes</button><h2>{detalhe.nomeCompleto}</h2><p>{detalhe.descricao}</p><p>Público-alvo: {detalhe.publicoAlvo}</p>{detalhe.modulos.map(m=><div key={m.id}><h3>{m.titulo}</h3><ul>{m.aulas.map(a=><li key={a.id}><strong>{a.titulo}:</strong> {a.descricao}</li>)}</ul></div>)}</section>}
 </main><footer>Projeto demonstrativo — dados fictícios</footer></>;
}
