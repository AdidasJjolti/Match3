using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quest;


namespace Match3.Board
{
    public class Block : MonoBehaviour
    {
        #region TypeCheck
        [SerializeField] protected _eBlockType _blockType;
        public _eBlockType type
        {
            get
            {
                return _blockType;
            }

            set
            {
                _blockType = value;
            }
        }

        bool _init = false;
        public Block(_eBlockType blockType)
        {
            // 생성자 함수로 생성하면 Init 함수 호출하지 않음
            _init = true;
            _blockType = blockType;

            _status = _eBlockStatus.NORMAL;
            _questType = _eBlockQuestType.CLEAR_SIMPLE;
            _match = _eMatchType.NONE;
            _breed = _eBlockBreed.NONE;

            _durability = 1;
        }

        public void Init(_eBlockType blockType)
        {
            if(_init)
            {
                return;
            }

            _init = true;
            _blockType = blockType;

            _status = _eBlockStatus.NORMAL;
            _questType = _eBlockQuestType.CLEAR_SIMPLE;
            _match = _eMatchType.NONE;
            _breed = _eBlockBreed.NONE;

            _durability = 1;
        }
        #endregion

        #region BlockBehaviour
        [SerializeField] protected BlockBehaviour _blockBehaviour;
        public BlockBehaviour blockBehaviour
        {
            get
            {
                return _blockBehaviour;
            }
            
            set
            {
                _blockBehaviour = value;
                //_blockBehaviour.SetBlock(this);
            }
        }

        internal void Move(float x, float y)
        {
            this.transform.position = new Vector3(x, y);
        }
        #endregion


        #region BreedCheck
        public _eBlockBreed _breed;
        public _eBlockBreed breed
        {
            get
            {
                return _breed;
            }

            set
            {
                _breed = value;
                //_blockBehaviour?.UpdateView(true);      // breed 값이 set되는 경우 블럭 스프라이트 변경
            }
        }

        // Block에 연결된 게임 오브젝트의 트랜스폼 반환
        public Transform blockObj
        {
            get
            {
                return transform;
            }
        }

        Vector2Int _vtDuplicate;  // 블럭 중복 갯수, 셔플할 때 중복 검사에 사용

        // 가로 방향 중복 검사에 사용
        public int horzDuplicate
        {
            get
            {
                return _vtDuplicate.x;
            }
            set
            {
                _vtDuplicate.x = value;
            }
        }

        // 세로 방향 중복 검사에 사용
        public int vertDuplicate
        {
            get
            {
                return _vtDuplicate.y;
            }
            set
            {
                _vtDuplicate.y = value;
            }
        }

        public void ResetDuplicationInfo()
        {
            _vtDuplicate.x = 0;
            _vtDuplicate.y = 0;
        }

        // 같은 종류의 breed인지 검사
        public bool IsEqual(Block target)
        {
            if(IsMatchableBlock() && this.breed == target.breed)
            {
                return true;
            }

            return false;
        }
        #endregion

        #region SwipeAction
        // 3매치 대상이 되는 블럭인지 검사, EMPTY 타입이 아닌 블럭 검사
        public bool IsMatchableBlock()
        {
            return (type == _eBlockType.BASIC);
        }

        // to의 위치로 블럭을 이동하는 메서드
        public void MoveTo(Vector3 to, float duration)
        {
            _blockBehaviour.StartCoroutine(Util.Action2D.MoveTo(blockObj, to, duration));
        }

        public bool IsSwipeable(Block baseBlock)
        {
            return true;
        }
        #endregion

        #region MatchCheck

        public _eBlockStatus _status;
        public _eBlockQuestType _questType;
        public _eMatchType _match = _eMatchType.NONE;
        public short _matchCount;

        [SerializeField] int _durability;
        public virtual int durability
        {
            get
            {
                return _durability;
            }

            set
            {
                _durability = value;
            }
        }

        // 3매치가 성립하여 CLEAR 할 수 있는 경우 이후 동작을 수행
        public bool DoEvaluation(BoardEnumerator boardEnumerator, int nRow, int nCol)
        {
            Debug.Assert(boardEnumerator != null, $"({nRow},{nCol})");

            if(!IsEvaluatable())
            {
                return false;
            }

            // 매치 상태인 경우, CLEAR 할 수 있는 상태
            if(_status == _eBlockStatus.MATCH)
            {
                // 내구도 1 감소
                if(_questType == _eBlockQuestType.CLEAR_SIMPLE || boardEnumerator.IsCageTypeCell(nRow, nCol))
                {
                    Debug.Assert(_durability > 0, $"durability is zero : {_durability}");
                    _durability--;
                }
                else
                {
                    return true;
                }

                // 내구도가 0이 되면 CLEAR 상태로 변경
                if (_durability == 0)
                {
                    _status = _eBlockStatus.CLEAR;
                    return false;
                }
            }

            // 내구도가 0보다 크면 NORMAL 상태로 변경하여 CLEAR 할 수 없음
            _status = _eBlockStatus.NORMAL;
            _match = _eMatchType.NONE;
            _matchCount = 0;

            return false;
        }

        public void UpdateBlockStatusMatched(_eMatchType matchType, bool accumulate = true)
        {
            this._status = _eBlockStatus.MATCH;

            if(_match == _eMatchType.NONE)
            {
                this._match = matchType;
            }
            else
            {
                this._match = accumulate ? _match.Add(matchType) : matchType;       // _match의 정수값과 matchType 정수값을 더하여 _match에 새로운 _eMatchType 값을 할당
            }
        }

        public bool IsEvaluatable()
        {
            // 이미 클리어 처리 되었거나 현재 처리중인 블럭인 경우 false 반환
            if(_status == _eBlockStatus.CLEAR || !IsMatchableBlock())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 유효한 블럭인지 체크한다.
        /// EMPTY 타입을 제외하고 모든 블럭이 유효한 것으로 간주한다.
        /// Block GameObject 생성 등의 판단에 사용된다.
        /// </summary>
        /// <returns></returns>
        public bool IsValidate()
        {
            return type != _eBlockType.EMPTY;
        }

        public virtual void Destroy()
        {
            Debug.Assert(blockObj != null, $"{_match}");
            _blockBehaviour.DoActionClear();
        }

        [SerializeField] BlockActionBehaviour _blockActionBehaviour;
        public bool _isMoving
        {
            get
            {
                return blockObj != null && _blockActionBehaviour._isMoving;
            }
        }

        public Vector2 _dropDistance
        {
            set
            {
                _blockActionBehaviour?.MoveDrop(value);
            }
        }
        #endregion
    }
}

