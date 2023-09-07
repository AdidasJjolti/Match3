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
            // ������ �Լ��� �����ϸ� Init �Լ� ȣ������ ����
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
                //_blockBehaviour?.UpdateView(true);      // breed ���� set�Ǵ� ��� �� ��������Ʈ ����
            }
        }

        // Block�� ����� ���� ������Ʈ�� Ʈ������ ��ȯ
        public Transform blockObj
        {
            get
            {
                return transform;
            }
        }

        Vector2Int _vtDuplicate;  // �� �ߺ� ����, ������ �� �ߺ� �˻翡 ���

        // ���� ���� �ߺ� �˻翡 ���
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

        // ���� ���� �ߺ� �˻翡 ���
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

        // ���� ������ breed���� �˻�
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
        // 3��ġ ����� �Ǵ� ������ �˻�, EMPTY Ÿ���� �ƴ� �� �˻�
        public bool IsMatchableBlock()
        {
            return (type == _eBlockType.BASIC);
        }

        // to�� ��ġ�� ���� �̵��ϴ� �޼���
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

        // 3��ġ�� �����Ͽ� CLEAR �� �� �ִ� ��� ���� ������ ����
        public bool DoEvaluation(BoardEnumerator boardEnumerator, int nRow, int nCol)
        {
            Debug.Assert(boardEnumerator != null, $"({nRow},{nCol})");

            if(!IsEvaluatable())
            {
                return false;
            }

            // ��ġ ������ ���, CLEAR �� �� �ִ� ����
            if(_status == _eBlockStatus.MATCH)
            {
                // ������ 1 ����
                if(_questType == _eBlockQuestType.CLEAR_SIMPLE || boardEnumerator.IsCageTypeCell(nRow, nCol))
                {
                    Debug.Assert(_durability > 0, $"durability is zero : {_durability}");
                    _durability--;
                }
                else
                {
                    return true;
                }

                // �������� 0�� �Ǹ� CLEAR ���·� ����
                if (_durability == 0)
                {
                    _status = _eBlockStatus.CLEAR;
                    return false;
                }
            }

            // �������� 0���� ũ�� NORMAL ���·� �����Ͽ� CLEAR �� �� ����
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
                this._match = accumulate ? _match.Add(matchType) : matchType;       // _match�� �������� matchType �������� ���Ͽ� _match�� ���ο� _eMatchType ���� �Ҵ�
            }
        }

        public bool IsEvaluatable()
        {
            // �̹� Ŭ���� ó�� �Ǿ��ų� ���� ó������ ���� ��� false ��ȯ
            if(_status == _eBlockStatus.CLEAR || !IsMatchableBlock())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// ��ȿ�� ������ üũ�Ѵ�.
        /// EMPTY Ÿ���� �����ϰ� ��� ���� ��ȿ�� ������ �����Ѵ�.
        /// Block GameObject ���� ���� �Ǵܿ� ���ȴ�.
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

