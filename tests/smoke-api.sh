#!/usr/bin/env bash
# Testes funcionais de contrato HTTP do catálogo.
# Uso: bash tests/smoke-api.sh http://127.0.0.1:5080
set -euo pipefail
base="${1:-http://127.0.0.1:5080}"
base="${base%/}"

assert_json() {
  local url="$1" expression="$2" label="$3"
  echo "TESTE: $label"
  curl --fail --silent --show-error --max-time 25 "$base$url" | jq -e "$expression" >/dev/null
  echo "  OK"
}
assert_json "/health" '.status=="ok"' "health"
assert_json "/api/cursos" '.total==6 and (.itens|length)==6' "seis cursos"
assert_json "/api/filtros" '(.categorias|length)==3 and (.modalidades|length)==2 and (.status|length)==3' "3 categorias, 2 modalidades, 3 status"
assert_json "/api/cursos?categoria=Matem%C3%A1tica" '.total==2' "categoria Matemática"
assert_json "/api/cursos?categoria=Linguagens" '.total==2' "categoria Linguagens"
assert_json "/api/cursos?modalidade=Online" '.total==3' "modalidade Online"
assert_json "/api/cursos?status=Dispon%C3%ADvel" '.total==3' "status Disponível"
assert_json "/api/cursos?pesquisa=Matem%C3%A1tica" '.total==1' "pesquisa por nome"
assert_json "/api/cursos?pesquisa=Matem%C3%A1tica&categoria=Matem%C3%A1tica&modalidade=Online&status=Dispon%C3%ADvel" '.total==1' "filtros cumulativos"
assert_json "/api/cursos?pesquisa=Matem%C3%A1tica&categoria=Linguagens" '.total==0 and (.itens|length)==0' "combinação sem resultados"
assert_json "/api/cursos/1" '(.id==1) and (.modulos|length)==2 and ([.modulos[].aulas|length]|add)==3' "detalhes, 2 módulos, 3 aulas"
code="$(curl --silent --show-error -o /dev/null -w '%{http_code}' "$base/api/cursos/99999")"
if [[ "$code" != "404" ]]; then
  echo "ERRO: curso inexistente deveria retornar 404, retornou $code" >&2
  exit 1
fi
echo "TESTE: curso inexistente 404 - OK"
echo "12 cenários de API aprovados."
