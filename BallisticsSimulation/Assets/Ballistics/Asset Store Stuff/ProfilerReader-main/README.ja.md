# ProfilerReader
�v���t�@�C���[�̃��O�t�@�C������͂���c�[���ł�<br />
Read this in other languages: [English](README.md), ���{��<br />

## �T�v
���̃c�[���� "Unity Profiler".
For example, generate csv files that shows Samples allocating many Managed Heap from "Unity Profiler" binary log.

## �Ή��o�[�W����
2019.4 / 2020.3/2021.1/2021.2/2021.3/2022.2/2022.3
<br/>2022.1 skip

## Filter�����@�\
�uTools->UTJ->ProfilerReader->AnalyzeToCsv�v�ŉ��L�E�B���h�E���o�܂��B<br />
![alt text](Documentation~/img/ProfilerReaderFilter.png)
<br />
1.�v���t�@�C���[�̃��O���w�肵�܂�<br />
2.���������T���v���̏������w�肵�܂�<br />
3.�����ɍ����T���v���������܂�<br />
4.�����ɍ������T���v���̌��ʂ�\�����܂�<br />
5.���ʂ�CSV�ɏ����o���܂��B<br />

<br />

## CSV���@�\
### GUI����ɂ���
�uTools->UTJ->ProfilerReader->AnalyzeToCsv�v�ŉ��L�E�B���h�E���o�܂��B<br />
![alt text](Documentation~/img/ProfilerLogToCsv.png)

<br />
����Window�ł�CSV�����ꂽ�T�}���[�𐶐����܂��B<br />
1.�����o�����s����ʂ̎w����s���܂��B<br />
2.Profiler�̃��O�t�@�C�����w�肵�܂��B<br />
3.��͂����s���܂�<br />

### CUI����ł̃T���v��
Unity.exe -batchMode -projectPath "ProjectPath" -logFile .\Editor.log -executeMethod UTJ.ProfilerReader.CUIInterface.ProfilerToCsv -PH.inputFile "Binary logFile(.data/.raw)" -PH.timeout 2400 -PH.log

�o�C�i�����O�t�@�C���Ɠ����ꏊ�ɃT�u�t�H���_���쐬���A�������CSV�t�@�C������������܂��B

## CSV Files:
CS�t�@�C���́A���̃t�@�C�����Ƀt�b�^�[��t�����t�@�C�����ŏo�͂��܂��B<br />
���L�̂悤�Ȍ`��CSV�͂����������o���܂�<br />

���uxxx_mainThread_frame.csv�v<br />
�t���[�����̃��C���X���b�h�ł̃J�e�S���ʂ�CPU�������ׂ̃��X�g
<br />
���uxxx_gc_result.csv�v<br />
�S�̂�ʂ���GC�������m�ی��̃��X�g
<br />
���uxxx_gpu_sample.csv�v<br />
Profiler��GPU���ڂ��t���[�����ɃJ�e�S���ʂɏo����郊�X�g
<br />
���uxxx_main_self.csv�v<br />
�S�̂�ʂ��ăT���v���ʂ�CPU���׃��X�g
<br />
���uxxx_memory.csv�v<br />
Profiler��Memory���ڂ��t���[�����ɏo�������X�g
<br />
���uxxx_rendering.csv�v<br />
Profiler��Rendering���ڂ��t���[�����ɏo�������X�g
<br />
���uxxx_renderthread.csv�v<br />
RenderThread�̏󋵂��t���[�����ɏo�������X�g
<br />
���uxxx_result.csv�v<br />
�t���[������Thread�̏󋵃��X�g
<br />
���uxxx_shader_compile.csv�v<br />
Shader�R���p�C�����������󋵂������o�����X�g
<br />
���uxxx_urp_gpu_sample.csv�v<br />
GPU���ڂ�Universal RP�����ɏ����o�������X�g
<br />
���uxxx_worker.csv�v<br />
WorkerThread�̏󋵂������o�������X�g
<br />

